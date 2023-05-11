using Domain.TransactionalEvents;

namespace Infrastructure.MongoDb.TransactionalEvents.Models {
    public class TransactionalEventsGroup {
        public string Id { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset AvailableDate { get; set; }
        public List<TransactionalEvent> TransactionalEvents { get; set; }
    }
}
