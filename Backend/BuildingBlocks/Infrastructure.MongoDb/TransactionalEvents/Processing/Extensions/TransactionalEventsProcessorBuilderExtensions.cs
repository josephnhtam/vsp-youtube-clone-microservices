using Infrastructure.TransactionalEvents.Processing;
using Infrastructure.TransactionalEvents.Processing.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.MongoDb.TransactionalEvents.Processing.Extensions {
    public static class TransactionalEventsProcessorBuilderExtensions {

        public static ITransactionalEventsProcessorBuilder UseMongoDb (this ITransactionalEventsProcessorBuilder builder) {
            builder.Services.TryAddTransient<ITransactionalEventsProcessor, TransactionalEventsProcessor>();
            return builder;
        }

    }
}
