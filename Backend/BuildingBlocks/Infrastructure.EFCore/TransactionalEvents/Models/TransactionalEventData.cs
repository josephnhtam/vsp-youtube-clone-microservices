using Domain.TransactionalEvents;

namespace Infrastructure.EFCore.TransactionalEvents.Models {
    public class TransactionalEventData {
        public long Id { get; set; }
        public string GroupId { get; set; }
        public long SequenceNumber { get; set; }
        public TransactionalEvent Event { get; set; }
    }
}
