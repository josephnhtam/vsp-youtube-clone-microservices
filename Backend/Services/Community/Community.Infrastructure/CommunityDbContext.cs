using Community.Domain.Models;
using Infrastructure.EFCore.Idempotency.Extensions;
using Infrastructure.EFCore.TransactionalEvents.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Community.Infrastructure {
    public class CommunityDbContext : DbContext {

        public DbSet<VideoForum> VideoForums { get; set; }
        public DbSet<VideoComment> VideoComments { get; set; }
        public DbSet<VideoCommentVote> VideoCommentVotes { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        public CommunityDbContext (DbContextOptions<CommunityDbContext> options) : base(options) {
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddTransactionalEventsModels();
            modelBuilder.AddIdempotentOperationModel();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommunityDbContext).Assembly);
        }

    }
}
