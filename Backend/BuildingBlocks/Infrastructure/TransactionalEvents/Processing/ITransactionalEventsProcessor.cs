namespace Infrastructure.TransactionalEvents.Processing {
    public interface ITransactionalEventsProcessor {
        Task ProcessTransactionalEvents (CancellationToken cancellationToken = default);
    }
}
