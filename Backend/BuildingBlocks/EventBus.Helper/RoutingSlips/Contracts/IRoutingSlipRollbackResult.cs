namespace EventBus.Helper.RoutingSlips.Contracts {
    public interface IRoutingSlipRollbackResult {
        Task ExecuteAsync (IServiceProvider serviceProvider, IIncomingIntegrationEventProperties eventProperties, IIncomingIntegrationEventContext eventContext, CancellationToken cancellationToken = default);
    }
}
