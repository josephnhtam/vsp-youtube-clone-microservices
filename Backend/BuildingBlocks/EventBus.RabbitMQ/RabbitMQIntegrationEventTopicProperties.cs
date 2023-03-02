namespace EventBus.RabbitMQ {
    public class RabbitMQIntegrationEventTopicProperties : IIntegrationEventTopicProperties {
        /// <summary>
        /// Default: Type name of the Topic without namespace
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// TopicName is the alias of ExchangeName
        /// </summary>
        public string ExchangeName { get { return TopicName; } set { TopicName = value; } }

        /// <summary>
        /// Default: fanout
        /// </summary>
        public string ExchangeType { get; set; }

        /// <summary>
        /// Default: true
        /// </summary>
        public bool Durable { get; set; }

        /// <summary>
        /// Default: false
        /// </summary>
        public bool AutoDelete { get; set; }

        /// <summary>
        /// Default: Empty
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; }
    }
}
