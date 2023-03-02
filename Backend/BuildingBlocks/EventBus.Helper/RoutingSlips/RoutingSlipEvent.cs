namespace EventBus.Helper.RoutingSlips {
    public class RoutingSlipEvent : IntegrationEvent {
        public RoutingSlipEventType Type { get; set; }
        public int CheckpointIndex { get; set; }
        public RoutingSlip RoutingSlip { get; set; }

        public RoutingSlipEvent (RoutingSlipEventType type, int checkpointIndex, RoutingSlip routingSlip) {
            Type = type;
            CheckpointIndex = checkpointIndex;
            RoutingSlip = routingSlip;
        }
    }
}
