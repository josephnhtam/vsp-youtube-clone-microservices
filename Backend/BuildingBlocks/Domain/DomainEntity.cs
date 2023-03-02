using Domain.Events;
using Domain.Exceptions;
using Domain.Rules;

namespace Domain {
    public abstract class DomainEntity : Entity, IDomainEventEmitter {

        private List<IDomainEvent> _domainEvents = new();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public DomainEntity () { }

        public void AddDomainEvent (IDomainEvent domainEvent) {
            lock (_domainEvents) {
                _domainEvents.Add(domainEvent);
            }
        }

        public void AddUniqueDomainEvent (IDomainEvent domainEvent) {
            lock (_domainEvents) {
                _domainEvents.RemoveAll(x => {
                    return x.GetType() == domainEvent.GetType();
                });
                _domainEvents.Add(domainEvent);
            }
        }

        public void RemoveAllDomainEvents () {
            lock (_domainEvents) {
                _domainEvents.Clear();
            }
        }

        public void RemoveDomainEvent (IDomainEvent domainEvent) {
            lock (_domainEvents) {
                _domainEvents.Remove(domainEvent);
            }
        }

        public void CheckRule<TRule> (TRule rule) where TRule : IBusinessRule {
            if (rule.IsBroken()) {
                throw BusinessRuleValidationException.Create(rule);
            }
        }

        public void CheckRule (IBusinessRule rule) {
            if (rule.IsBroken()) {
                throw BusinessRuleValidationException.Create(rule);
            }
        }

        public void CheckRules (params IBusinessRule[] rules) {
            foreach (var rule in rules) {
                CheckRule(rule);
            }
        }

    }
}
