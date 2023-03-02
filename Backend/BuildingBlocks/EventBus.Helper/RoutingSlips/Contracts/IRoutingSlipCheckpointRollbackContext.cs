namespace EventBus.Helper.RoutingSlips.Contracts {
    public interface IRoutingSlipCheckpointRollbackContext {
        IRoutingSlip RoutingSlip { get; }
        IRoutingSlipRollbackResult Complete (string? message = null, IDictionary<string, string>? variables = null, Action<IIntegrationEventProperties>? configureEvent = null);
        IRoutingSlipRollbackResult Terminate ();
    }
}
