using Domain.Events;

namespace Infrastructure.MongoDb.DomainEventsDispatching {
    public class DomainEventsAccessor : IDomainEventsAccessor {

        private readonly IDomainEventEmittersTracker? _emitterTracker;

        public DomainEventsAccessor (IServiceProvider services) {
            _emitterTracker = services.GetService(typeof(IDomainEventEmittersTracker)) as IDomainEventEmittersTracker;
        }

        public void ClearDomainEvents () {
            if (_emitterTracker != null) {
                foreach (var domainEventEmitter in _emitterTracker.DomainEventEmitters) {
                    domainEventEmitter.RemoveAllDomainEvents();
                }
            }
        }

        public IReadOnlyList<IDomainEvent> GetDomainEvents () {
            if (_emitterTracker != null) {
                var result = _emitterTracker.DomainEventEmitters.SelectMany(x => x.DomainEvents).ToList();
                return result;
            }

            return new List<IDomainEvent>();
        }

    }
}
