using EventBus.Helper.RoutingSlips.Contracts;

namespace EventBus.Helper.RoutingSlips {
    public class RoutingSlipProceedRollbackedResult : IRoutingSlipProceedResult {
        public Task ExecuteAsync (IServiceProvider serviceProvider, IIncomingIntegrationEventProperties eventProperties, IIncomingIntegrationEventContext eventContext, CancellationToken cancellationToken = default) {
            return Task.CompletedTask;
        }
    }
}
