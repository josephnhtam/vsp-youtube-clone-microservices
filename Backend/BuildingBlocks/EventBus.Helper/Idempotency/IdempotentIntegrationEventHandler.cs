using Domain.Contracts;
using Infrastructure.Idempotency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventBus.Helper.Idempotency {
    public abstract class IdempotentIntegrationEventHandler<TIntegrationEvent, TQueue> :
        IntegrationEventHandler<TIntegrationEvent, TQueue>
        where TIntegrationEvent : IntegrationEventBase
        where TQueue : IntegrationEventQueue {

        private readonly IIdempotencyContext _idempotencyContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IdempotentIntegrationEventHandler<TIntegrationEvent, TQueue>> _logger;

        public IdempotentIntegrationEventHandler (IServiceProvider serviceProvider) {
            _idempotencyContext = serviceProvider.GetRequiredService<IIdempotencyContext>();
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            _logger = serviceProvider.GetRequiredService<ILogger<IdempotentIntegrationEventHandler<TIntegrationEvent, TQueue>>>();
        }

        public sealed override async Task Handle (TIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _unitOfWork.ExecuteTransactionAsync(async () => {
                var integrationEventId = integrationEvent.Id;
                var operationId = integrationEventId.ToString();

                if (await _idempotencyContext.IsOperationIdStoredAsync(operationId) ||
                    !await _idempotencyContext.StoreOperationIdAsync(operationId)) {
                    _logger.LogWarning("Idempotent event ({IntegrationEvent_Id}) has already been handled.", integrationEventId);
                    return;
                }

                _logger.LogInformation("Handling Idempotent event ({IntegrationEvent_Id}).", integrationEventId);

                await HandleIdempotently(integrationEvent, properties, context);
            });
        }

        public abstract Task HandleIdempotently (TIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context);

    }

    public abstract class IdempotentIntegrationEventHandler<TIntegrationEvent> :
        IdempotentIntegrationEventHandler<TIntegrationEvent, IntegrationEventQueue>
        where TIntegrationEvent : IntegrationEventBase {
        protected IdempotentIntegrationEventHandler (IServiceProvider serviceProvider) : base(serviceProvider) { }
    }
}
