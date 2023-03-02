using Domain.Events;

namespace Library.Domain.DomainEvents.Videos {
    public class VideoVoteMetricsChangedDomainEvent : IDomainEvent {
        public Guid VideoId { get; set; }
        public long LikesCountChange { get; set; }
        public long DislikesCountChange { get; set; }
        public DateTimeOffset? NextSyncDate { get; set; }

        public VideoVoteMetricsChangedDomainEvent (Guid videoId, long likesCountChange, long dislikesCountChange, DateTimeOffset? nextSyncDate) {
            VideoId = videoId;
            LikesCountChange = likesCountChange;
            DislikesCountChange = dislikesCountChange;
            NextSyncDate = nextSyncDate;
        }
    }
}
