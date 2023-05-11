using Domain.TransactionalEvents;

namespace Infrastructure.TransactionalEvents.Processing {
    public interface ITransactionalEventsHandler {
        Task ProcessTransactionalEventsAsync (List<TransactionalEvent> events, CancellationToken cancellationToken);
    }
}
