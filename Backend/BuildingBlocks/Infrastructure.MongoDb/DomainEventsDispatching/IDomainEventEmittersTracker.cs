using Domain.Events;

namespace Infrastructure.MongoDb.DomainEventsDispatching {
    public interface IDomainEventEmittersTracker {
        IReadOnlyList<IDomainEventEmitter> DomainEventEmitters { get; }
        void Track (IDomainEventEmitter domainEventEmitter);
        void Untrack (IDomainEventEmitter domainEventEmitter);
        void Reset ();
    }
}
