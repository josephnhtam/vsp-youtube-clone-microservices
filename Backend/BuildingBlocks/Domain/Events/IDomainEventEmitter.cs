namespace Domain.Events {
    public interface IDomainEventEmitter {
        public void AddDomainEvent (IDomainEvent domainEvent);
        public void RemoveDomainEvent (IDomainEvent domainEvent);
        public void RemoveAllDomainEvents ();
        public IReadOnlyList<IDomainEvent> DomainEvents { get; }
    }
}
