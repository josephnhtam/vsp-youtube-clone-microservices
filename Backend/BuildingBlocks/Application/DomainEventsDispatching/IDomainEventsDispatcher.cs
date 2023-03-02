namespace Application.DomainEventsDispatching {
    public interface IDomainEventsDispatcher {
        Task DispatchDomainEventsAsync ();
    }
}
