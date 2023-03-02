using Domain.Events;

namespace Infrastructure.MongoDb.DomainEventsDispatching {
    public class DomainEventEmittersTracker : IDomainEventEmittersTracker {

        private List<IDomainEventEmitter> _domainEventEmitters;

        public IReadOnlyList<IDomainEventEmitter> DomainEventEmitters => _domainEventEmitters.AsReadOnly();

        public DomainEventEmittersTracker () {
            _domainEventEmitters = new List<IDomainEventEmitter>();
        }

        public void Reset () {
            lock (_domainEventEmitters) {
                _domainEventEmitters.Clear();
            }
        }

        public void Track (IDomainEventEmitter domainEventEmitter) {
            lock (_domainEventEmitters) {
                if (!_domainEventEmitters.Contains(domainEventEmitter)) {
                    _domainEventEmitters.Add(domainEventEmitter);
                }
            }
        }

        public void Untrack (IDomainEventEmitter domainEventEmitter) {
            lock (_domainEventEmitters) {
                _domainEventEmitters.Remove(domainEventEmitter);
            }
        }
    }
}
