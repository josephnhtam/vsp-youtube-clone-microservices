using Infrastructure.EFCore.Idempotency.Extensions;
using Infrastructure.EFCore.TransactionalEvents.Extensions;
using Microsoft.EntityFrameworkCore;
using Subscriptions.Domain.Models;

namespace Subscriptions.Infrastructure {
    public class SubscriptionsDbContext : DbContext {

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        public SubscriptionsDbContext (DbContextOptions<SubscriptionsDbContext> options) : base(options) {
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddTransactionalEventsModels();
            modelBuilder.AddIdempotentOperationModel();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SubscriptionsDbContext).Assembly);
        }

    }
}
