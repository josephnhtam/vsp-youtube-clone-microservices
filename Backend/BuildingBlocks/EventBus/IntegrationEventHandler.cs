using Serilog.Context;
using Serilog.Core.Enrichers;
using SharedKernel.Utilities;
using System.Diagnostics;

namespace EventBus {
    public abstract class IntegrationEventHandlerBase {

        protected static readonly ActivitySource ActivitySource = new ActivitySource("IntegrationEvent");

        internal IntegrationEventHandlerBase () { }

        public abstract Task Handle (IntegrationEventBase integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context);
        public abstract Task HandleFailure (IntegrationEventBase integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context, Exception ex);
    }

    public abstract class IntegrationEventHandler<TIntegrationEvent, TQueue> : IntegrationEventHandlerBase
        where TIntegrationEvent : IntegrationEventBase
        where TQueue : IntegrationEventQueue {

        private static readonly string _integrationEventName;

        static IntegrationEventHandler () {
            _integrationEventName = typeof(TIntegrationEvent).GetGenericTypeName();
        }

        public sealed override Task Handle (IntegrationEventBase integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            using var activity = ActivitySource.StartActivity(
                $"{_integrationEventName}.Handle",
                ActivityKind.Internal,
                default(ActivityContext),
                new List<KeyValuePair<string, object?>> {
                    new ("IntegrationEvent_Id", integrationEvent.Id),
                    new ("IntegrationEvent_CreateDate", integrationEvent.CreateDate),
                });

            using var logContext = LogContext.Push(
                new PropertyEnricher("IntegrationEvent_Id", integrationEvent.Id),
                new PropertyEnricher("IntegrationEvent_CreateDate", integrationEvent.CreateDate)
            );

            return Handle((integrationEvent as TIntegrationEvent)!, properties, context);
        }

        public sealed override Task HandleFailure (IntegrationEventBase integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context, Exception ex) {
            using var activity = ActivitySource.StartActivity(
                $"{_integrationEventName}.HandleFailure",
                ActivityKind.Internal,
                default(ActivityContext),
                new List<KeyValuePair<string, object?>> {
                    new ("IntegrationEvent_Id", integrationEvent.Id),
                    new ("IntegrationEvent_CreateDate", integrationEvent.CreateDate),
                });

            using var logContext = LogContext.Push(
                new PropertyEnricher("IntegrationEvent_Id", integrationEvent.Id),
                new PropertyEnricher("IntegrationEvent_CreateDate", integrationEvent.CreateDate)
            );

            return HandleFailure((integrationEvent as TIntegrationEvent)!, properties, context, ex);
        }

        public abstract Task Handle (TIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context);
        public virtual Task HandleFailure (TIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context, Exception ex) {
            return Task.CompletedTask;
        }
    }

    public abstract class IntegrationEventHandler<TIntegrationEvent> : IntegrationEventHandler<TIntegrationEvent, IntegrationEventQueue>
        where TIntegrationEvent : IntegrationEventBase { }
}
