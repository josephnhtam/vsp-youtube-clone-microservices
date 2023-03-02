using Infrastructure.EFCore.Idempotency.Extensions;
using Infrastructure.EFCore.Interceptors;
using Infrastructure.EFCore.TransactionalEvents.Extensions;
using Microsoft.EntityFrameworkCore;
using VideoManager.Domain.Models;

namespace VideoManager.Infrastructure {
    public class VideoManagerDbContext : DbContext {

        public DbSet<Video> Videos { get; set; }
        public DbSet<ProcessedVideo> ProcessedVideos { get; set; }
        public DbSet<VideoThumbnail> VideoThumbnails { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        public VideoManagerDbContext (DbContextOptions<VideoManagerDbContext> options) : base(options) {
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddTransactionalEventsModels();
            modelBuilder.AddIdempotentOperationModel();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VideoManagerDbContext).Assembly);
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.AddInterceptors(new NpgsqlPessimisticLockCommandInterceptor());
        }

    }
}
