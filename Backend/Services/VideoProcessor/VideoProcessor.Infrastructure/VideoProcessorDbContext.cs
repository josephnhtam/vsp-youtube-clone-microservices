using Infrastructure.EFCore.Idempotency.Extensions;
using Infrastructure.EFCore.Interceptors;
using Infrastructure.EFCore.TransactionalEvents.Extensions;
using Microsoft.EntityFrameworkCore;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Infrastructure {
    public class VideoProcessorDbContext : DbContext {

        public DbSet<Video> Videos { get; set; }
        public DbSet<ProcessedVideo> ProcessedVideos { get; set; }
        public DbSet<VideoThumbnail> VideoThumbnails { get; set; }

        public VideoProcessorDbContext (DbContextOptions<VideoProcessorDbContext> options) : base(options) {
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddTransactionalEventsModels();
            modelBuilder.AddIdempotentOperationModel();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VideoProcessorDbContext).Assembly);
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.AddInterceptors(new NpgsqlPessimisticLockCommandInterceptor());
        }

    }
}
