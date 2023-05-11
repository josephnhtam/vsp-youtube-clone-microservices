namespace Domain.TransactionalEvents.Contracts {
    public interface ITransactionalEventsContext {
        IServiceProvider ServiceProvider { get; }
        void AddEvent (TransactionalEvent message);
        void AddEvent (string? groupId, TransactionalEvent message);
        bool HasPendingEvent ();
        void ClearUncommitedEvents ();
        void ResetDefaultEventsGroudId ();
    }
}
