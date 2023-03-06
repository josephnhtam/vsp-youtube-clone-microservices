using Domain;
using Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EFCore.DomainEventsDispatching {
    public class DomainEventsAccessor<TDbContext> : IDomainEventsAccessor where TDbContext : DbContext {

        private readonly TDbContext _dbContext;

        public DomainEventsAccessor (TDbContext dbContext) {
            _dbContext = dbContext;
        }

        public void ClearDomainEvents () {
            var eventEmitters = _dbContext.ChangeTracker
                .Entries<Entity>()
                .Select(x => x.Entity)
                .OfType<IDomainEventEmitter>();

            foreach (var eventEmitter in eventEmitters) {
                eventEmitter.RemoveAllDomainEvents();
            }
        }

        public IReadOnlyList<IDomainEvent> GetDomainEvents () {
            return _dbContext.ChangeTracker
                .Entries<Entity>()
                .Select(x => x.Entity)
                .OfType<IDomainEventEmitter>()
                .SelectMany(x => x.DomainEvents).ToList();
        }

    }
}
