namespace EventBus.Helper.RoutingSlips.Contracts {
    public interface IRoutingSlipCheckpointProceedContext {
        IRoutingSlip RoutingSlip { get; }
        IRoutingSlipProceedResult Complete (string? message = null, IDictionary<string, string>? variables = null, Action<IIntegrationEventProperties>? configureEvent = null);
        IRoutingSlipProceedResult Rollback (string? message = null, IDictionary<string, string>? variables = null, Action<IIntegrationEventProperties>? configureEvent = null);
        IRoutingSlipProceedResult Terminate ();
    }
}
