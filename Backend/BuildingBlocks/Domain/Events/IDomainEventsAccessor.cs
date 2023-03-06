namespace Domain.Events {
    public interface IDomainEventsAccessor {
        IReadOnlyList<IDomainEvent> GetDomainEvents ();
        void ClearDomainEvents ();
    }
}
