using Infrastructure.EFCore.Idempotency.Extensions;
using Infrastructure.EFCore.TransactionalEvents.Extensions;
using Microsoft.EntityFrameworkCore;
using Storage.Domain.Models;

namespace Storage.Infrastructure {
    public class StorageDbContext : DbContext {

        public DbSet<StoredFile> Files { get; set; }
        public DbSet<FileTracking> FileTrackings { get; set; }


        public StorageDbContext (DbContextOptions<StorageDbContext> options) : base(options) {
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddTransactionalEventsModels();
            modelBuilder.AddIdempotentOperationModel();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StorageDbContext).Assembly);
        }

    }
}
