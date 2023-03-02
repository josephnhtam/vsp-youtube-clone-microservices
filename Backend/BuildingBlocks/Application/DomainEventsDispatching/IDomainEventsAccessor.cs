using Domain.Events;

namespace Application.DomainEventsDispatching {
    public interface IDomainEventsAccessor {
        IReadOnlyList<IDomainEvent> GetDomainEvents ();
        void ClearDomainEvents ();
    }
}
