namespace EventBus.RabbitMQ {
    public class RabbitMQIntegrationEventQueueProperties : IIntegrationEventQueueProperties {
        /// <summary>
        /// Default: Type name of the Queue without namespace
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Default: QueueName.dead-letter
        /// </summary>
        public string? DeadLetterQueueName { get; set; }

        /// <summary>
        /// Default: Empty
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// Should this queue will survive a broker restart?
        /// Default: true
        /// </summary>
        public bool Durable { get; set; }

        /// <summary>
        /// Should this queue use be limited to its declaring connection?
        /// Such a queue will be deleted when its declaring connection closes.
        /// Default: false
        /// </summary>
        public bool Exclusive { get; set; }

        /// <summary>
        /// Should this queue be auto-deleted when its last consumer (if any) unsubscribes?
        /// Default: false
        /// </summary>
        public bool AutoDelete { get; set; }

        /// <summary>
        /// Default: false
        /// </summary>
        public bool RequeueEventWhenNack { get; set; }

        /// <summary>
        /// Default: 0
        /// </summary>
        public uint PrefetchSize { get; set; }

        /// <summary>
        /// Default: 32
        /// </summary>
        public ushort PrefetchCount { get; set; }

        /// <summary>
        /// Optional; additional queue arguments, e.g. "x-queue-type"
        /// Default: Empty
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; }

        /// <summary>
        /// Optional; additional queue binding arguments, e.g. "x-match"
        /// Default: Empty
        /// </summary>
        public IDictionary<string, object> BindingArguments { get; set; }
    }
}
