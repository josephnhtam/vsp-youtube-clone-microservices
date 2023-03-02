using EventBus.Helper.Idempotency;
using EventBus.Helper.RoutingSlips.Contracts;
using Serilog.Context;
using Serilog.Core.Enrichers;
using System.Text.Json;

namespace EventBus.Helper.RoutingSlips {
    public abstract class RoutingSlipCheckpointHandler<TCheckpointProperties, TEventQueue> : IdempotentIntegrationEventHandler<RoutingSlipEvent, TEventQueue>
        where TCheckpointProperties : class, new()
        where TEventQueue : RoutingSlipEventQueue {

        private readonly IServiceProvider _serviceProvider;

        private static JsonSerializerOptions JsonSerializerOptions =
            new JsonSerializerOptions {
                IncludeFields = true
            };

        public RoutingSlipCheckpointHandler (IServiceProvider serviceProvider) : base(serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public sealed override async Task HandleIdempotently (RoutingSlipEvent @event, IIncomingIntegrationEventProperties eventProperties, IIncomingIntegrationEventContext eventContext) {
            var routingSlip = @event.RoutingSlip;
            var checkpoint = @event.CheckpointIndex >= 0 ? routingSlip.Checkpoints[@event.CheckpointIndex] : routingSlip.RollbackDestination;
            var properties = checkpoint?.PropertiesData != null ? JsonSerializer.Deserialize<TCheckpointProperties>(checkpoint.PropertiesData, JsonSerializerOptions) : null;

            using var logContext = LogContext.Push(
                new PropertyEnricher("RoutingSlip_Id", routingSlip.Id),
                new PropertyEnricher("RoutingSlip_Type", @event.Type.ToString()),
                new PropertyEnricher("RoutingSlip_CheckpointIndex", @event.CheckpointIndex)
            );

            if (@event.Type == RoutingSlipEventType.Proceed) {
                IRoutingSlipCheckpointProceedContext context = new RoutingSlipCheckpointProceedContext(@event);
                var result = await HandleProceed(properties!, context, eventProperties, eventContext);
                await result.ExecuteAsync(_serviceProvider, eventProperties, eventContext);
            } else {
                IRoutingSlipCheckpointRollbackContext context = new RoutingSlipCheckpointRollbackContext(@event);
                var result = await HandleRollback(properties!, context, eventProperties, eventContext);
                await result.ExecuteAsync(_serviceProvider, eventProperties, eventContext);
            }
        }

        public sealed override async Task HandleFailure (RoutingSlipEvent @event, IIncomingIntegrationEventProperties eventProperties, IIncomingIntegrationEventContext eventContext, Exception ex) {
            using var logContext = LogContext.Push(
                new PropertyEnricher("RoutingSlip_Id", @event.RoutingSlip.Id),
                new PropertyEnricher("RoutingSlip_Type", @event.Type.ToString()),
                new PropertyEnricher("RoutingSlip_CheckpointIndex", @event.CheckpointIndex)
            );

            if (@event.Type == RoutingSlipEventType.Proceed) {
                IRoutingSlipCheckpointProceedContext context = new RoutingSlipCheckpointProceedContext(@event);
                var result = context.Rollback(ex.ToString());
                await result.ExecuteAsync(_serviceProvider, eventProperties, eventContext);
            } else {
                IRoutingSlipCheckpointRollbackContext context = new RoutingSlipCheckpointRollbackContext(@event);
                var result = context.Terminate();
                await result.ExecuteAsync(_serviceProvider, eventProperties, eventContext);
            }
        }

        protected virtual Task<IRoutingSlipProceedResult> HandleProceed (TCheckpointProperties properties, IRoutingSlipCheckpointProceedContext context, IIncomingIntegrationEventProperties eventProperties, IIncomingIntegrationEventContext eventContext) {
            return Task.FromResult(context.Complete());
        }

        protected virtual Task<IRoutingSlipRollbackResult> HandleRollback (TCheckpointProperties properties, IRoutingSlipCheckpointRollbackContext context, IIncomingIntegrationEventProperties eventProperties, IIncomingIntegrationEventContext eventContext) {
            return Task.FromResult(context.Complete());
        }

    }
}
