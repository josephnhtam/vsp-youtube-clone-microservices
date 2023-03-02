namespace EventBus.RabbitMQ.Exceptions {
    public class IntegrationEventHandlerMissingException : Exception {

        public string EventTypeName { get; }

        public IntegrationEventHandlerMissingException (string eventTypeName) {
            EventTypeName = eventTypeName;
        }

        public override string ToString () {
            return $"The integration event handler for {EventTypeName} is not registered";
        }

    }
}
