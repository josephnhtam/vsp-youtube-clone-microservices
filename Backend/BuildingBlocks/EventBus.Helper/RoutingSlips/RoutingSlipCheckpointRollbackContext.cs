using EventBus.Helper.RoutingSlips.Contracts;

namespace EventBus.Helper.RoutingSlips {
    public class RoutingSlipCheckpointRollbackContext : IRoutingSlipCheckpointRollbackContext {

        private readonly RoutingSlipEvent _event;

        public IRoutingSlip RoutingSlip => _event.RoutingSlip;

        public RoutingSlipCheckpointRollbackContext (RoutingSlipEvent @event) {
            _event = @event;
        }

        public IRoutingSlipRollbackResult Complete (string? message = null, IDictionary<string, string>? variables = null, Action<IIntegrationEventProperties>? configureEvent = null) {
            var nextCheckpointIndex = _event.CheckpointIndex - 1;

            if (nextCheckpointIndex == -1 && RoutingSlip.RollbackDestination == null) {
                return new RoutingSlipRollbackCompleteResult();
            } else if (nextCheckpointIndex < -1) {
                return new RoutingSlipRollbackCompleteResult();
            }

            return new RoutingSlipRollbackResult(
                UpdateRoutingSlip(message, variables),
                nextCheckpointIndex,
                configureEvent);
        }

        private RoutingSlip UpdateRoutingSlip (string? message, IDictionary<string, string>? variables) {
            var builder = new RoutingSlipBuilder(_event);

            builder.AddCheckpointLog(true, message);
            if (variables != null) builder.SetVariables(variables);
            var routingSlip = builder.Build();
            return routingSlip;
        }

        public IRoutingSlipRollbackResult Terminate () {
            return new RoutingSlipRollbackTerminatedResult();
        }

    }
}
