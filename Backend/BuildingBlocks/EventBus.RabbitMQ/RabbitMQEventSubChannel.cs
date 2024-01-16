using EventBus.RabbitMQ.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace EventBus.RabbitMQ {
    public class RabbitMQEventSubChannel {

        private readonly IServiceProvider _serviceProvider;
        private readonly IDeadLetterEventBus _deadLetterEventBus;
        private readonly IConnection _connection;
        private readonly IRabbitMQTopology _topology;
        private readonly ILogger<RabbitMQEventSubChannel> _logger;
        private readonly RabbitMQEventBusConfiguration _configuration;
        private readonly CancellationToken _connectionAborted;
        private readonly ILoggerFactory _loggerFactory;

        private static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions {
            IncludeFields = true,
            WriteIndented = false
        };

        public RabbitMQEventSubChannel (IServiceProvider serviceProvider, IDeadLetterEventBus deadLetterEventBus, IConnection connection, IRabbitMQTopology topology, RabbitMQEventBusConfiguration configuration, ILogger<RabbitMQEventSubChannel> logger, ILoggerFactory loggerFactory, CancellationToken connectionAborted) {
            _serviceProvider = serviceProvider;
            _deadLetterEventBus = deadLetterEventBus;
            _connection = connection;
            _topology = topology;
            _logger = logger;
            _configuration = configuration;
            _loggerFactory = loggerFactory;
            _connectionAborted = connectionAborted;
        }

        public async Task RunChannel () {
            while (!_connectionAborted.IsCancellationRequested) {
                IModel? channel = null;

                try {
                    CreateChannel(out channel, out CancellationToken channelAborted);
                    StartConsume(channel);

                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(_connectionAborted, channelAborted);

                    try {
                        await Task.Delay(Timeout.Infinite, cts.Token);
                    } finally {
                        cts.Cancel();
                    }
                } catch (OperationCanceledException) {
                    _logger.LogInformation("Sub channel is stopping");
                } catch (Exception ex) {
                    _logger.LogError(ex, "An error has occurred when running sub channel");
                }

                if (channel != null) {
                    await DisposeChannelAsync(channel);
                    channel = null;
                }

                if (!_connectionAborted.IsCancellationRequested) {
                    _logger.LogInformation("Restarting sub channel");
                }
            }
        }

        private async Task DisposeChannelAsync (IModel channel) {
            await Task.Run(() => DisposeChannel(channel));
        }

        private void DisposeChannel (IModel channel) {
            if (channel.IsOpen) channel.Close();
            channel.Dispose();
        }

        private void CreateChannel (out IModel channel, out CancellationToken channelAborted) {
            var channelAbortedSource = new CancellationTokenSource();
            channelAborted = channelAbortedSource.Token;

            channel = _connection.CreateModel();
            channel.ConfirmSelect();

            channel.CallbackException += OnCallbackException(channelAbortedSource);

            channel.BasicQos(_configuration.Qos.PrefetchSize, _configuration.Qos.PrefetchCount, true);
            _topology.DeclareTopology(channel, _serviceProvider, _logger);
        }

        private void StartConsume (IModel channel) {
            var queueDefinitions = _topology.GetEventQueueDefinitions();

            foreach (var queueDefinition in queueDefinitions) {
                var consumer = new AsyncEventingBasicConsumer(channel);

                var eventConsumer = new EventConsumer(
                    _deadLetterEventBus,
                    _topology,
                    queueDefinition,
                    _serviceProvider,
                    _configuration,
                    channel,
                    _loggerFactory.CreateLogger<EventConsumer>());

                consumer.Received += eventConsumer.ConsumeEvent;

                channel.BasicQos(queueDefinition.PrefetchSize, queueDefinition.PrefetchCount, false);
                channel.BasicConsume(queueDefinition.QueueName, false, consumer);
            }
        }

        private EventHandler<CallbackExceptionEventArgs> OnCallbackException (CancellationTokenSource channelAbortedSource) {
            return (object? sender, CallbackExceptionEventArgs e) => {
                _logger.LogWarning(e.Exception, "Callback exception occurred");
                channelAbortedSource.Cancel();
            };
        }

        private class EventConsumer {

            private readonly IDeadLetterEventBus _deadLetterEventBus;
            private readonly IRabbitMQTopology _topology;
            private readonly EventQueueDefinition _queueDefinition;

            private readonly IServiceProvider _serviceProvider;
            private readonly RabbitMQEventBusConfiguration _configuration;
            private readonly IModel _channel;
            private readonly ILogger<EventConsumer> _logger;

            public EventConsumer (IDeadLetterEventBus deadLetterEventBus, IRabbitMQTopology topology, EventQueueDefinition queueDefinition, IServiceProvider serviceProvider, RabbitMQEventBusConfiguration configuration, IModel channel, ILogger<EventConsumer> logger) {
                _deadLetterEventBus = deadLetterEventBus;
                _topology = topology;
                _queueDefinition = queueDefinition;

                _serviceProvider = serviceProvider;
                _configuration = configuration;
                _channel = channel;
                _logger = logger;
            }

            public async Task ConsumeEvent (object? sender, BasicDeliverEventArgs eventArgs) {
                var context = new RabbitMQIncomingIntegrationEventContext();

                try {
                    var body = JsonSerializer.Deserialize(Encoding.UTF8.GetString(eventArgs.Body.Span), typeof(RabbitMQIntegrationEvent), JsonSerializerOptions) as RabbitMQIntegrationEvent;
                    if (body == null) throw new SerializationException($"Failed to deserialize event from queue ({_queueDefinition.QueueName})");

                    var eventAndHandlerType = _topology.GetEventAndHandlerType(_queueDefinition.QueueType, body.Type);
                    if (eventAndHandlerType == null) {
                        throw new Exception("Handler is not registered");
                    }

                    var eventType = eventAndHandlerType.Value.eventType;
                    var eventHandlerType = eventAndHandlerType.Value.eventHandlerType;

                    var @event = (JsonSerializer.Deserialize(body.Data, eventType, JsonSerializerOptions) as IntegrationEventBase)!;

                    using var scope = _serviceProvider.CreateScope();

                    var eventHandler = scope.ServiceProvider.GetService(eventHandlerType) as IntegrationEventHandlerBase;
                    if (eventHandler == null) throw new IntegrationEventHandlerMissingException(eventType.Name);

                    var properties = new RabbitMQIncomingIntegrationEventProperties {
                        Exchange = eventArgs.Exchange,
                        RoutingKey = eventArgs.RoutingKey,
                        Redelivered = eventArgs.Redelivered,
                        Persistent = eventArgs.BasicProperties.Persistent
                    };

                    _logger.LogInformation("Consuming event ({Type}) ({IntegrationEvent_Id}) from queue ({QueueName})", body.Type, @event.Id, _queueDefinition.QueueName);

                    bool ShouldRequeue (Exception ex) {
                        return context.RequeueWhenNack ?? _configuration.RequeuePolicies.Any(x => x.ShouldRequeue(ex));
                    }

                    try {
                        var policy = Policy.Handle<Exception>(ShouldRequeue)
                                           .WaitAndRetryAsync(
                                               _configuration.Subscribing.MaxLocalRetryCount,
                                               (attempt) => TimeSpan.FromSeconds(Math.Pow(2, attempt))
                                            );

                        await policy.ExecuteAsync(async () => {
                            await eventHandler.Handle(@event, properties, context);
                            _channel.BasicAck(eventArgs.DeliveryTag, false);
                        });
                    } catch (Exception ex) {
                        TimeSpan requeueDelay = _configuration.Subscribing.MaxLocalRetryCount > 0 ?
                            TimeSpan.Zero :
                            TimeSpan.FromSeconds(2);

                        if (ShouldRequeue(ex)) {
                            _logger.LogError(ex,
                                "An error is occurred when consuming an event. The event will be requeued.");
                            Requeue(eventArgs.DeliveryTag, requeueDelay);
                        } else {
                            try {
                                _logger.LogError(ex,
                                    "An error is occurred when consuming an event. The event will be moved to dead-letter queue.");

                                await eventHandler.HandleFailure(@event, properties, context, ex);
                                await MoveEventToDeadLetterQueue(eventArgs, ex);
                            } catch (Exception fex) {
                                _logger.LogError(fex, "An error is occurred when moving an event to dead-leter queue. The event will be requeued.");
                                Requeue(eventArgs.DeliveryTag, requeueDelay);
                            }
                        }
                    }
                } catch (Exception ex) {
                    await MoveEventToDeadLetterQueue(eventArgs, ex);

                    _logger.LogError(ex,
                            "An error is occurred when deserializing an event. " +
                            "The event will be moved to dead-letter queue.");
                }
            }

            private async Task MoveEventToDeadLetterQueue (BasicDeliverEventArgs eventArgs, Exception failure) {
                bool success = true;

                try {
                    await _deadLetterEventBus.PublishDeadLetterEvent(_queueDefinition.DeadLetterQueueName, eventArgs.Body, failure);
                } catch (Exception ex) {
                    _logger.LogError(ex, "Failed to move the event to dead-letter queue");
                    success = false;
                } finally {
                    _channel.BasicNack(eventArgs.DeliveryTag, false, !success);
                }
            }

            private async void Requeue (ulong deliveryTag, TimeSpan delay) {
                await Task.Delay(delay);
                _channel.BasicNack(deliveryTag, false, true);
            }

        }

    }
}
