using Infrastructure.TransactionalEvents.Processing;
using Infrastructure.TransactionalEvents.Processing.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.EFCore.TransactionalEvents.Processing.Extensions {
    public static class TransactionalEventsProcessorBuilderExtensions {

        public static ITransactionalEventsProcessorBuilder UseEntityFrameworkCore<TDbContext> (this ITransactionalEventsProcessorBuilder builder)
            where TDbContext : DbContext {
            builder.Services.TryAddTransient<ITransactionalEventsProcessor, TransactionalEventsProcessor<TDbContext>>();
            return builder;
        }

    }
}
