namespace EventBus.Helper.RoutingSlips.Contracts {
    public interface IRoutingSlip {
        Guid Id { get; }
        IReadOnlyList<IRoutingSlipCheckpoint> Checkpoints { get; }
        IReadOnlyList<IRoutingSlipCheckpointLog> CheckpointLogs { get; }
        IRoutingSlipCheckpoint? RollbackDestination { get; }
        IReadOnlyDictionary<string, string> Blackboard { get; }
    }
}