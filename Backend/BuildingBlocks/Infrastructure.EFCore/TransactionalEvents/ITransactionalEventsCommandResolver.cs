using Infrastructure.EFCore.TransactionalEvents.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EFCore.TransactionalEvents {
    public interface ITransactionalEventsCommandResolver {
        IQueryable<TransactionalEventsGroup> PollEventsGroups (DbContext context, int fetchCount);
        Task<long> UpsertEventsGroupAndGetLastSequenceNumber (DbContext context, string groupId, int eventsCount, DateTimeOffset currentTime, TimeSpan availableDelay);
    }
}
