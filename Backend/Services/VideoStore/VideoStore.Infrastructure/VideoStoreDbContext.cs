using Infrastructure.EFCore.Idempotency.Extensions;
using Infrastructure.EFCore.Interceptors;
using Infrastructure.EFCore.TransactionalEvents.Extensions;
using Microsoft.EntityFrameworkCore;
using VideoStore.Domain.Models;

namespace VideoStore.Infrastructure {
    public class VideoStoreDbContext : DbContext {

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Video> Videos { get; set; }

        public VideoStoreDbContext (DbContextOptions<VideoStoreDbContext> options) : base(options) {
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddTransactionalEventsModels();
            modelBuilder.AddIdempotentOperationModel();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VideoStoreDbContext).Assembly);
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.AddInterceptors(new NpgsqlPessimisticLockCommandInterceptor());
        }

    }
}
