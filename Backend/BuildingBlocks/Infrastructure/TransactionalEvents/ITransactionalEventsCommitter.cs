using Domain.TransactionalEvents;

namespace Infrastructure.TransactionalEvents {
    public interface ITransactionalEventsCommitter {
        Dictionary<string, List<TransactionalEvent>> ObtainEventGroups ();
        void RemoveFromContext (Dictionary<string, List<TransactionalEvent>> eventGroups);
        Task AddToContextAsync (Dictionary<string, List<TransactionalEvent>> eventGroups, TimeSpan? availableDelay = null, CancellationToken cancellationToken = default);
    }
}
