namespace EventBus {
    public abstract class IntegrationEventBase {

        public Guid Id { get; init; }
        public DateTimeOffset CreateDate { get; init; }

        internal IntegrationEventBase () {
            Id = Guid.NewGuid();
            CreateDate = DateTimeOffset.UtcNow;
        }

    }

    public abstract class IntegrationEvent<TTopic> : IntegrationEventBase where TTopic : IntegrationEventTopic { }

    public abstract class IntegrationEvent : IntegrationEventBase { }
}
