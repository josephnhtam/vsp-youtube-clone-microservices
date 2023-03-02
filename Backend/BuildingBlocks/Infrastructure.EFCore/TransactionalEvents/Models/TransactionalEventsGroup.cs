namespace Infrastructure.EFCore.TransactionalEvents.Models {
    public class TransactionalEventsGroup {
        public string Id { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset AvailableDate { get; set; }
        public long LastSequenceNumber { get; set; }
        public List<TransactionalEventData> TransactionalEvents { get; set; }
    }
}
