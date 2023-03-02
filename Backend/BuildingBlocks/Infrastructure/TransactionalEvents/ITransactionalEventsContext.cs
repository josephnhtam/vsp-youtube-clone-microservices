namespace Infrastructure.TransactionalEvents {
    public interface ITransactionalEventsContext {
        IServiceProvider ServiceProvider { get; }
        void AddEvent (TransactionalEvent message);
        void AddEvent (string? groupId, TransactionalEvent message);
        bool HasPendingEvent ();
        void ClearUncommitedEvents ();
        void ResetDefaultEventsGroudId ();
    }
}
