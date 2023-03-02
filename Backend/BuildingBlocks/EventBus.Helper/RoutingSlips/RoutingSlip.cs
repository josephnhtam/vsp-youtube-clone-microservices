using EventBus.Helper.RoutingSlips.Contracts;

namespace EventBus.Helper.RoutingSlips {
    public class RoutingSlip : IRoutingSlip {
        public Guid Id { get; set; }
        public List<RoutingSlipCheckpoint> Checkpoints { get; set; }
        public List<RoutingSlipCheckpointLog> CheckpointLogs { get; set; }
        public RoutingSlipCheckpoint? RollbackDestination { get; set; }
        public Dictionary<string, string> Blackboard { get; set; }

        Guid IRoutingSlip.Id => Id;
        IReadOnlyList<IRoutingSlipCheckpoint> IRoutingSlip.Checkpoints => Checkpoints.AsReadOnly();
        IReadOnlyList<IRoutingSlipCheckpointLog> IRoutingSlip.CheckpointLogs => CheckpointLogs.AsReadOnly();
        IRoutingSlipCheckpoint? IRoutingSlip.RollbackDestination => RollbackDestination;
        IReadOnlyDictionary<string, string> IRoutingSlip.Blackboard => Blackboard;

        public RoutingSlip (Guid id, List<RoutingSlipCheckpoint> checkpoints, List<RoutingSlipCheckpointLog> checkpointLogs, RoutingSlipCheckpoint? rollbackDestination, Dictionary<string, string> blackboard) {
            Id = id;
            Checkpoints = checkpoints;
            CheckpointLogs = checkpointLogs;
            RollbackDestination = rollbackDestination;
            Blackboard = blackboard;
        }
    }
}
