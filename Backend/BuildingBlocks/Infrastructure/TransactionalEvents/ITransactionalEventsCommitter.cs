namespace Infrastructure.TransactionalEvents {
    public interface ITransactionalEventsCommitter {
        Dictionary<string, List<TransactionalEvent>> ObtainEventGroups ();
        Task CommitEventsAsync (Dictionary<string, List<TransactionalEvent>> eventGroups, TimeSpan? availableDelay = null, CancellationToken cancellationToken = default);
    }
}
