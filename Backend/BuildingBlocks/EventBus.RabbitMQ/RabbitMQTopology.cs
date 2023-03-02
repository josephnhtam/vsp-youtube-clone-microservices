using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SharedKernel.Utilities;

namespace EventBus.RabbitMQ {
    public class RabbitMQTopology : IRabbitMQTopology {

        private readonly IReadOnlyList<Type> _eventTypes;
        private readonly IReadOnlyList<Type> _eventHandlerTypes;

        private readonly Dictionary<Type, EventTopicDefinition> _topicDefinitionByEventType;
        private readonly Dictionary<Type, EventQueueDefinition> _queueDefinitionByHandlerType;

        private readonly Dictionary<Type, EventTopicDefinition> _topicDefinitionByTopicType;
        private readonly Dictionary<Type, EventQueueDefinition> _queueDefinitionsByQueueType;
        private readonly Dictionary<(Type eventQueueType, string eventTypeName), (Type eventType, Type eventHandlerType)> _eventTypeAndHandlerDict;

        private readonly Dictionary<Type, Type> _eventTypeByHandlerType;

        public RabbitMQTopology (IReadOnlyList<Type> integrationEventTypes, IReadOnlyList<Type> integrationEventHandlerTypes) {
            _eventTypes = integrationEventTypes;
            _eventHandlerTypes = integrationEventHandlerTypes;

            _topicDefinitionByEventType = new Dictionary<Type, EventTopicDefinition>();
            _queueDefinitionByHandlerType = new Dictionary<Type, EventQueueDefinition>();

            _topicDefinitionByTopicType = new Dictionary<Type, EventTopicDefinition>();
            _queueDefinitionsByQueueType = new Dictionary<Type, EventQueueDefinition>();
            _eventTypeAndHandlerDict = new Dictionary<(Type eventQueueType, string eventTypeName), (Type eventType, Type eventHandlerType)>();

            _eventTypeByHandlerType = new Dictionary<Type, Type>();
        }

        public void DeclareTopology (IModel channel, IServiceProvider services, ILogger logger) {
            foreach (var eventType in _eventTypes) {
                RegisterIntegrationEvent(services, eventType);
            }

            foreach (var eventHandlerType in _eventHandlerTypes) {
                RegisterIntegrationEventHandler(services, eventHandlerType);
            }

            foreach (var eventDefinition in _topicDefinitionByEventType.Values) {
                try {
                    channel.ExchangeDeclare(
                        eventDefinition.ExchangeName,
                        eventDefinition.ExchangeType,
                        eventDefinition.Durable,
                        eventDefinition.AutoDelete,
                        eventDefinition.Arguments);
                } catch (Exception ex) {
                    logger.LogError(ex, "Failed to declare exchange");
                }
            }

            foreach (var queueDefinition in _queueDefinitionByHandlerType.Values) {
                try {
                    channel.QueueDeclare(
                        queueDefinition.QueueName,
                        queueDefinition.Durable,
                        queueDefinition.Exclusive,
                        queueDefinition.AutoDelete,
                        queueDefinition.Arguments);

                    channel.QueueDeclare(
                        queueDefinition.DeadLetterQueueName,
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());
                } catch (Exception ex) {
                    logger.LogError(ex, "Failed to declare queue");
                }
            }

            Dictionary<EventQueueDefinition, List<EventTopicDefinition>> topicDefinitionsByQueueDefinition
                = new Dictionary<EventQueueDefinition, List<EventTopicDefinition>>();

            foreach ((var handlerType, var eventType) in _eventTypeByHandlerType) {
                var queueDefinition = _queueDefinitionByHandlerType[handlerType];

                if (!topicDefinitionsByQueueDefinition.TryGetValue(queueDefinition, out var topicDefinitions)) {
                    topicDefinitions = new List<EventTopicDefinition>();
                    topicDefinitionsByQueueDefinition[queueDefinition] = topicDefinitions;
                }

                if (_topicDefinitionByEventType.TryGetValue(eventType, out var topicDefinition)) {
                    if (!topicDefinitions.Contains(topicDefinition)) {
                        topicDefinitions.Add(topicDefinition);
                    }
                }
            }

            foreach ((var queueDefinition, var topicDefinitions) in topicDefinitionsByQueueDefinition) {
                foreach (var topicDefinition in topicDefinitions) {
                    try {
                        channel.QueueBind(queueDefinition.QueueName, topicDefinition.ExchangeName,
                            queueDefinition.RoutingKey, queueDefinition.BindingArguments);
                    } catch (Exception ex) {
                        logger.LogError(ex, "Failed to bind queue with exchange");
                    }
                }
            }
        }

        private void RegisterIntegrationEvent (IServiceProvider services, Type eventType) {
            if (_topicDefinitionByEventType.ContainsKey(eventType)) return;

            Type? topicType = GetEventTopicType(eventType);

            if (topicType != null) {
                if (!_topicDefinitionByTopicType.TryGetValue(topicType, out var topicDefinition)) {
                    topicDefinition = CreateEventTopicDefinition(services, topicType);
                    _topicDefinitionByTopicType[topicType] = topicDefinition;
                }

                _topicDefinitionByEventType[eventType] = topicDefinition;
            }
        }

        private EventTopicDefinition CreateEventTopicDefinition (IServiceProvider services, Type topicType) {
            var topic = (Activator.CreateInstance(topicType) as IntegrationEventTopic)!;
            var properties = new RabbitMQIntegrationEventTopicProperties() {
                ExchangeName = topicType.GetGenericTypeName(),
                ExchangeType = "fanout",
                Durable = true,
                AutoDelete = false,
                Arguments = new Dictionary<string, object>()
            };

            topic.OnTopicCreating(services, properties);

            var definition = new EventTopicDefinition() {
                ExchangeName = properties.ExchangeName,
                ExchangeType = properties.ExchangeType,
                Durable = properties.Durable,
                AutoDelete = properties.AutoDelete,
                Arguments = properties.Arguments
            };

            return definition;
        }

        private Type? GetEventTopicType (Type integrationEvent) {
            Type eventType = integrationEvent;

            while (!eventType.IsGenericType || eventType.GetGenericTypeDefinition() != typeof(IntegrationEvent<>)) {
                if (eventType.BaseType != null) {
                    eventType = eventType.BaseType;
                } else {
                    return null;
                }
            }

            return eventType.GetGenericArguments()[0];
        }

        private void RegisterIntegrationEventHandler (IServiceProvider services, Type handlerType) {
            if (_queueDefinitionByHandlerType.ContainsKey(handlerType)) return;

            (var eventType, var queueType) = GetTypesFromHandler(handlerType);

            if (!_queueDefinitionsByQueueType.TryGetValue(queueType, out var queueDefinition)) {
                queueDefinition = CreateEventQueueDefinition(services, queueType);
                _queueDefinitionsByQueueType[queueType] = queueDefinition;
            }

            _eventTypeByHandlerType[handlerType] = eventType;
            _queueDefinitionByHandlerType[handlerType] = queueDefinition;
            _eventTypeAndHandlerDict[(queueType, eventType.GetGenericTypeName())] = (eventType, handlerType);
        }

        private EventQueueDefinition CreateEventQueueDefinition (IServiceProvider services, Type queueType) {
            var queue = (Activator.CreateInstance(queueType) as IntegrationEventQueue)!;

            var properties = new RabbitMQIntegrationEventQueueProperties() {
                QueueName = queueType.GetGenericTypeName(),
                RoutingKey = string.Empty,
                Durable = true,
                Exclusive = false,
                AutoDelete = false,
                RequeueEventWhenNack = false,
                PrefetchSize = 0,
                PrefetchCount = 32,
                Arguments = new Dictionary<string, object>(),
                BindingArguments = new Dictionary<string, object>(),
            };

            queue.OnQueueCreating(services, properties);

            var definition = new EventQueueDefinition() {
                QueueType = queueType,
                QueueName = properties.QueueName,
                DeadLetterQueueName = properties.DeadLetterQueueName ?? properties.QueueName + ".dead-letter",
                RoutingKey = properties.RoutingKey,
                Durable = properties.Durable,
                Exclusive = properties.Exclusive,
                AutoDelete = properties.AutoDelete,
                PrefetchSize = properties.PrefetchSize,
                PrefetchCount = properties.PrefetchCount,
                Arguments = properties.Arguments,
                BindingArguments = properties.BindingArguments,
            };

            return definition;
        }

        private static (Type eventType, Type eventQueueType) GetTypesFromHandler (Type integrationEventHandlerType) {
            Type handlerType = integrationEventHandlerType;

            while (!handlerType.IsGenericType || handlerType.GetGenericTypeDefinition() != typeof(IntegrationEventHandler<,>)) {
                if (handlerType.BaseType != null) {
                    handlerType = handlerType.BaseType;
                } else {
                    throw new InvalidOperationException($"{integrationEventHandlerType} is not a IntegrationEventHandler<>");
                }
            }

            return (handlerType.GetGenericArguments()[0], handlerType.GetGenericArguments()[1]);
        }

        public EventTopicDefinition? GetTopicDefinition (Type eventType) {
            if (_topicDefinitionByEventType.TryGetValue(eventType, out var topicDefinition)) {
                return topicDefinition;
            }
            return null;
        }

        public IReadOnlyList<EventQueueDefinition> GetEventQueueDefinitions () {
            return _queueDefinitionByHandlerType.Values.ToList().AsReadOnly();
        }

        public (Type eventType, Type eventHandlerType)? GetEventAndHandlerType (Type queueType, string eventTypeName) {
            if (_eventTypeAndHandlerDict.TryGetValue((queueType, eventTypeName), out var result)) {
                return result;
            }
            return null;
        }
    }

    public class EventTopicDefinition {

        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }

    }

    public class EventQueueDefinition {

        public Type QueueType { get; set; }
        public string QueueName { get; set; }
        public string DeadLetterQueueName { get; set; }
        public string RoutingKey { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public uint PrefetchSize { get; set; }
        public ushort PrefetchCount { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
        public IDictionary<string, object> BindingArguments { get; set; }

    }
}