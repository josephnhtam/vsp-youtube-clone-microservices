using MediatR;

namespace Domain.Events {
    public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent {
    }
}
