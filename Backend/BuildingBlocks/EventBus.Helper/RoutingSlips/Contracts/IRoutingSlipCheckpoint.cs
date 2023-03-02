namespace EventBus.Helper.RoutingSlips.Contracts {
    public interface IRoutingSlipCheckpoint {
        string Name { get; }
        string Destination { get; }
        string PropertiesData { get; }
    }
}