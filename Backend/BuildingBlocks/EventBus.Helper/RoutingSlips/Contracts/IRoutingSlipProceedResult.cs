namespace EventBus.Helper.RoutingSlips.Contracts {
    public interface IRoutingSlipProceedResult {
        Task ExecuteAsync (IServiceProvider serviceProvider, IIncomingIntegrationEventProperties eventProperties, IIncomingIntegrationEventContext eventContext, CancellationToken cancellationToken = default);
    }
}
