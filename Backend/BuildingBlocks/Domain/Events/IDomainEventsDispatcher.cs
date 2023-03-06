namespace Domain.Events {
    public interface IDomainEventsDispatcher {
        Task DispatchDomainEventsAsync ();
    }
}
