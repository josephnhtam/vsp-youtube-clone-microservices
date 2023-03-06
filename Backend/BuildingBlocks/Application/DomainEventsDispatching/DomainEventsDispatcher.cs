using Domain.Events;
using MediatR;

namespace Application.DomainEventsDispatching {
    public class DomainEventsDispatcher : IDomainEventsDispatcher {

        private readonly IMediator _mediator;
        private readonly IDomainEventsAccessor? _domainEventsAccessor;

        public DomainEventsDispatcher (IMediator mediator, IServiceProvider serviceProvider) {
            _mediator = mediator;
            _domainEventsAccessor = serviceProvider.GetService(typeof(IDomainEventsAccessor)) as IDomainEventsAccessor;
        }

        public async Task DispatchDomainEventsAsync () {
            if (_domainEventsAccessor == null) return;

            var domainEvents = _domainEventsAccessor.GetDomainEvents();

            _domainEventsAccessor.ClearDomainEvents();

            foreach (var domainEvent in domainEvents) {
                await _mediator.Publish(domainEvent);
            }
        }

    }
}
