using EventBus.Helper.RoutingSlips.Contracts;

namespace EventBus.Helper.RoutingSlips {
    public class RoutingSlipCheckpointProceedContext : IRoutingSlipCheckpointProceedContext {

        private readonly RoutingSlipEvent _event;

        public IRoutingSlip RoutingSlip => _event.RoutingSlip;

        public RoutingSlipCheckpointProceedContext (RoutingSlipEvent @event) {
            _event = @event;
        }

        public IRoutingSlipProceedResult Complete (string? message = null, IDictionary<string, string>? variables = null, Action<IIntegrationEventProperties>? configureEvent = null) {
            var nextCheckpointIndex = _event.CheckpointIndex + 1;

            if (nextCheckpointIndex >= _event.RoutingSlip.Checkpoints.Count) {
                return new RoutingSlipProceedCompleteResult();
            }

            var builder = new RoutingSlipBuilder(_event);

            return new RoutingSlipProceedResult(
                RoutingSlipEventType.Proceed,
                UpdateRoutingSlip(true, message, variables),
                nextCheckpointIndex,
                configureEvent);
        }

        public IRoutingSlipProceedResult Rollback (string? message = null, IDictionary<string, string>? variables = null, Action<IIntegrationEventProperties>? configureEvent = null) {
            var nextCheckpointIndex = _event.CheckpointIndex - 1;

            if (nextCheckpointIndex < 0) {
                return new RoutingSlipProceedRollbackedResult();
            }

            return new RoutingSlipProceedResult(
                RoutingSlipEventType.Rollback,
                UpdateRoutingSlip(false, message, variables),
                nextCheckpointIndex,
                configureEvent);
        }

        private RoutingSlip UpdateRoutingSlip (bool success, string? message, IDictionary<string, string>? variables) {
            var builder = new RoutingSlipBuilder(_event);

            builder.AddCheckpointLog(success, message);
            if (variables != null) builder.SetVariables(variables);
            var routingSlip = builder.Build();
            return routingSlip;
        }

        public IRoutingSlipProceedResult Terminate () {
            return new RoutingSlipProceedTerminatedResult();
        }

    }
}
