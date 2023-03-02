namespace EventBus.Helper.RoutingSlips.Contracts {
    public interface IRoutingSlipCheckpointLog {
        string Name { get; }
        RoutingSlipEventType Type { get; }
        DateTimeOffset Date { get; }
        bool Success { get; }
        string? Message { get; }
    }
}
