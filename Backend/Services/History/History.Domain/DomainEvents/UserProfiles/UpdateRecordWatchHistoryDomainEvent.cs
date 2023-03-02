using Domain.Events;

namespace History.Domain.DomainEvents.UserProfiles {
    public class UpdateRecordWatchHistoryDomainEvent : IDomainEvent {
        public string Id { get; set; }
        public bool RecordWatchHistory { get; set; }

        public UpdateRecordWatchHistoryDomainEvent (string id, bool recordWatchHistory) {
            Id = id;
            RecordWatchHistory = recordWatchHistory;
        }
    }
}
