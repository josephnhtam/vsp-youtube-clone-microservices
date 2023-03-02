using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using SharedKernel.Exceptions;

namespace Infrastructure.EFCore {
    public static class DatabaseHelper {

        private static AsyncRetryPolicy retryPolicy = Policy
            .Handle<Exception>(ex => ex.Identify(ExceptionCategories.Transient))
            .WaitAndRetryForeverAsync((retryCount) => TimeSpan.FromSeconds(Math.Pow(2, Math.Min(8, retryCount))));

        public static async Task MigrateDatabase<TDbContext> (this IHost app)
            where TDbContext : DbContext {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var dbContext = services.GetRequiredService<TDbContext>();
            var logger = services.GetRequiredService<ILogger<TDbContext>>();

            await retryPolicy.ExecuteAsync(async () => {
                try {
                    logger.LogInformation("Trying to migrate Database ({DbContext})", typeof(TDbContext).Name);
                    await dbContext.Database.MigrateAsync();
                    logger.LogInformation("Database migration ({DbContext}) succeeds", typeof(TDbContext).Name);
                } catch (Exception ex) {
                    logger.LogError(ex, "Database migration ({DbContext}) failed", typeof(TDbContext).Name);
                    throw;
                }
            });
        }

        public static async Task SeedDatabase<TDbContext> (this IHost app, Func<TDbContext, IServiceProvider, ILogger<TDbContext>, Task> seedingTask)
            where TDbContext : DbContext {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<TDbContext>();
            var logger = services.GetRequiredService<ILogger<TDbContext>>();

            await retryPolicy.ExecuteAsync(async () => {
                try {
                    logger.LogInformation("Trying to seed Database ({DbContext})", typeof(TDbContext).Name);
                    await seedingTask.Invoke(context, services, logger);
                    logger.LogInformation("Database ({DbContext}) seeding succeeds", typeof(TDbContext).Name);
                } catch (Exception ex) {
                    logger.LogError(ex, "Database ({DbContext}) seeding failed", typeof(TDbContext).Name);
                    throw;
                }
            });
        }

    }
}
