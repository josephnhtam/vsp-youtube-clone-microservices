using Domain.Events;
using MediatR;

namespace Application.Handlers {
    public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent {
    }
}
