namespace EventBus.Helper.RoutingSlips.Contracts {
    public interface IRoutingSlipBuilder {
        Guid Id { get; }
        void AddCheckpoint (string name, string destination, object? properties = null);
        void SetRollbackDestination (string name, string destination, object? properties);
        void SetVariables (IEnumerable<KeyValuePair<string, string>> keyValuePairs);
        void SetVariable (string key, string value);
        void RemoveVariable (string key);
    }
}