using Domain.Contracts;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using EventBus.Helper.RoutingSlips.Contracts;
using EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Helper.RoutingSlips {
    public class RoutingSlipProceedResult : IRoutingSlipProceedResult {

        private readonly RoutingSlipEventType _type;
        private readonly RoutingSlip _routingSlip;
        private readonly int _nextCheckpointIndex;
        private readonly Action<IIntegrationEventProperties>? _configureEvent;

        public RoutingSlipProceedResult (RoutingSlipEventType type, RoutingSlip routingSlip, int nextCheckpointIndex, Action<IIntegrationEventProperties>? configureEvent) {
            _type = type;
            _routingSlip = routingSlip;
            _nextCheckpointIndex = nextCheckpointIndex;
            _configureEvent = configureEvent;
        }

        public async Task ExecuteAsync (IServiceProvider serviceProvider, IIncomingIntegrationEventProperties eventProperties, IIncomingIntegrationEventContext eventContext, CancellationToken cancellationToken = default) {
            var nextCheckpoint = _routingSlip.Checkpoints[_nextCheckpointIndex];

            var transactionalEventContext = serviceProvider.GetRequiredService<ITransactionalEventsContext>();
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

            var @event = new RoutingSlipEvent(_type, _nextCheckpointIndex, _routingSlip);

            transactionalEventContext.AddOutboxMessage(@event, (properties) => {
                _configureEvent?.Invoke(properties);

                if (properties is RabbitMQIntegrationEventProperties rmq) {
                    rmq.SkipExchange = true;
                    rmq.RoutingKey = nextCheckpoint.Destination;
                }
            });

            await unitOfWork.CommitAsync(cancellationToken);
        }

    }
}
