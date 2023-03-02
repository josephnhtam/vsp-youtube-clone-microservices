using EventBus.Helper.RoutingSlips.Contracts;
using System.Text.Json;

namespace EventBus.Helper.RoutingSlips {
    public class RoutingSlipBuilder : IRoutingSlipBuilder {

        private List<RoutingSlipCheckpoint> _checkpoints;
        private List<RoutingSlipCheckpointLog> _checkpointLogs;
        private RoutingSlipCheckpoint? _rollbackDestination;
        private Dictionary<string, string> _blackboard;

        private readonly RoutingSlipEvent? _event;
        public readonly Guid _id;

        Guid IRoutingSlipBuilder.Id => _id;

        internal RoutingSlipBuilder (Guid id) {
            _id = id;
            _checkpoints = new List<RoutingSlipCheckpoint>();
            _checkpointLogs = new List<RoutingSlipCheckpointLog>();
            _rollbackDestination = null;
            _blackboard = new Dictionary<string, string>();
        }

        internal RoutingSlipBuilder (RoutingSlipEvent routingSlipEvent) {
            _event = routingSlipEvent;

            var routingSlip = routingSlipEvent.RoutingSlip;
            _id = routingSlip.Id;
            _checkpoints = new List<RoutingSlipCheckpoint>(routingSlip.Checkpoints);
            _checkpointLogs = new List<RoutingSlipCheckpointLog>(routingSlip.CheckpointLogs);
            _rollbackDestination = routingSlip.RollbackDestination;
            _blackboard = new Dictionary<string, string>(routingSlip.Blackboard);
        }

        public void SetVariable (string key, string value) {
            _blackboard[key] = value;
        }

        public void RemoveVariable (string key) {
            _blackboard.Remove(key);
        }

        public void SetVariables (IEnumerable<KeyValuePair<string, string>> keyValuePairs) {
            foreach (var keyValuePair in keyValuePairs) {
                _blackboard[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        public void AddCheckpoint (string name, string destination, object? properties) {
            _checkpoints.Add(new RoutingSlipCheckpoint(name, destination,
                properties != null ? JsonSerializer.Serialize(properties, properties.GetType()) : null));
        }

        public void SetRollbackDestination (string name, string destination, object? properties) {
            _rollbackDestination = new RoutingSlipCheckpoint(name, destination,
                properties != null ? JsonSerializer.Serialize(properties, properties.GetType()) : null);
        }

        public void AddCheckpointLog (bool success, string? message) {
            if (_event != null) {
                var type = _event.Type;
                var checkpoint = _event.RoutingSlip.Checkpoints[_event.CheckpointIndex];

                _checkpointLogs.Add(new RoutingSlipCheckpointLog(
                    checkpoint.Name,
                    type,
                    DateTimeOffset.UtcNow,
                    success,
                    message));
            }
        }

        internal RoutingSlip Build () {
            return new RoutingSlip(
                _id,
                _checkpoints,
                _checkpointLogs,
                _rollbackDestination,
                _blackboard
            );
        }

    }
}
