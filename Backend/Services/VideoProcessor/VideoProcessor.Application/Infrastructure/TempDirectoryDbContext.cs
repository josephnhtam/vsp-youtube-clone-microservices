using Microsoft.EntityFrameworkCore;

namespace VideoProcessor.Application.Infrastructure {
    public class TempDirectoryDbContext : DbContext {

        public DbSet<TempDirectory> TempDirectories { get; set; }

        public TempDirectoryDbContext (DbContextOptions<TempDirectoryDbContext> options) : base(options) {
        }

    }
}
