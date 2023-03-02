using EventBus.RabbitMQ.Exceptions;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace EventBus.RabbitMQ {
    public class RabbitMQEventPubChannel {

        private readonly IServiceProvider _serviceProvider;
        private readonly IConnection _connection;
        private readonly IRabbitMQTopology _topology;
        private readonly RabbitMQEventBusConfiguration _configuration;
        private readonly IPendingEvents _pendingEvents;
        private readonly ILogger<RabbitMQEventPubChannel> _logger;
        private readonly CancellationToken _connectionAborted;
        private readonly ConcurrentDictionary<ulong, IPendingEvent> _outstandingConfirms;
        private readonly Policy _retryPolicy;

        public RabbitMQEventPubChannel (IServiceProvider serviceProvider, IPendingEvents pendingEvents, IConnection connection, IRabbitMQTopology topology, RabbitMQEventBusConfiguration configuration, ILogger<RabbitMQEventPubChannel> logger, CancellationToken connectionAborted) {
            _serviceProvider = serviceProvider;
            _connection = connection;
            _topology = topology;
            _configuration = configuration;
            _pendingEvents = pendingEvents;
            _logger = logger;
            _connectionAborted = connectionAborted;
            _outstandingConfirms = new ConcurrentDictionary<ulong, IPendingEvent>();

            _retryPolicy = Policy
                .Handle<BrokerUnreachableException>()
                .Or<ConnectFailureException>()
                .Or<OperationInterruptedException>()
                .Or<SocketException>()
                .Retry(_configuration.Publishing.MaxRetryCount);
        }

        public async Task RunChannel () {
            while (!_connectionAborted.IsCancellationRequested) {
                IModel? channel = null;

                try {
                    // Create a channel and get the cancellation token for channel shutdown 
                    CreateChannel(out channel, out CancellationToken channelAborted);

                    // Stop the channel due to either connection lost or channel shutdown
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(_connectionAborted, channelAborted);

                    await Run(channel, cts.Token).ConfigureAwait(false);
                } catch (Exception ex) {
                    _logger.LogError(ex, "An error has occurred when running pub channel");
                } finally {
                    ClearOutstandingConfirms();
                }

                if (channel != null) {
                    await DisposeChannelAsync(channel);
                    channel = null;
                }

                if (!_connectionAborted.IsCancellationRequested) {
                    _logger.LogInformation("Restarting pub channel");
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

            channel.BasicAcks += OnBasicAcks;
            channel.BasicNacks += OnBasicNacks;
            channel.CallbackException += OnCallbackException(channelAbortedSource);

            _topology.DeclareTopology(channel, _serviceProvider, _logger);
        }

        private EventHandler<CallbackExceptionEventArgs> OnCallbackException (CancellationTokenSource channelAbortedSource) {
            return (object? sender, CallbackExceptionEventArgs e) => {
                _logger.LogWarning(e.Exception, "Callback exception occurred");
                channelAbortedSource.Cancel();
            };
        }

        private void OnBasicAcks (object? sender, BasicAckEventArgs e) {
            if (e.Multiple) {
                var confirmed = _outstandingConfirms.Where(k => k.Key <= e.DeliveryTag);

                foreach (var entry in confirmed) {
                    ConfirmAck(entry.Key, true);
                }
            } else {
                ConfirmAck(e.DeliveryTag, true);
            }
        }

        private void OnBasicNacks (object? sender, BasicNackEventArgs e) {
            if (e.Multiple) {
                var confirmed = _outstandingConfirms.Where(k => k.Key <= e.DeliveryTag);

                foreach (var entry in confirmed) {
                    ConfirmAck(entry.Key, false);
                }
            } else {
                ConfirmAck(e.DeliveryTag, false);
            }
        }

        private void ConfirmAck (ulong sequenceNumber, bool acked) {
            if (_outstandingConfirms.TryRemove(sequenceNumber, out var pendingIntegrationEvent)) {
                if (acked) {
                    pendingIntegrationEvent.SetComplete();
                } else {
                    pendingIntegrationEvent.SetException(new RabbitMQNackException());
                }
            }
        }

        private void ClearOutstandingConfirms () {
            var outstandingConfirms = _outstandingConfirms.Values.ToList();
            _outstandingConfirms.Clear();

            outstandingConfirms.ForEach(x => x.SetException(new PublishChannelStoppedException()));
        }

        private async Task Run (IModel channel, CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                IPendingEvent? pendingEvent = null;
                try {
                    pendingEvent = await _pendingEvents.PollPendingEvent(cancellationToken).ConfigureAwait(false);

                    _retryPolicy.Execute(() => {
                        pendingEvent.Publish(channel, _topology, _logger, (seqNo) => {
                            _outstandingConfirms.TryAdd(seqNo, pendingEvent);
                        });
                    });
                } catch (Exception ex) when (ex is OperationCanceledException || ex is AlreadyClosedException) {
                    _logger.LogInformation("Pub channel is stopping");

                    if (pendingEvent != null) {
                        pendingEvent.SetException(new PublishChannelStoppedException());
                    }
                } catch (Exception ex) {
                    _logger.LogWarning(ex, "An error has occurred during event publishing");

                    if (pendingEvent != null) {
                        pendingEvent.SetException(ex);
                    }
                }
            }
        }

    }
}
