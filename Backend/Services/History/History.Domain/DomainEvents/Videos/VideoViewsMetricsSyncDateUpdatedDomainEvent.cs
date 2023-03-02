using Domain.Events;

namespace History.Domain.DomainEvents.Videos {
    public class VideoViewsMetricsSyncDateUpdatedDomainEvent : IDomainEvent {

        public Guid VideoId { get; set; }
        public DateTimeOffset? NextSyncDate { get; set; }

        public VideoViewsMetricsSyncDateUpdatedDomainEvent (Guid videoId, DateTimeOffset? nextViewsCountCollectDate) {
            VideoId = videoId;
            NextSyncDate = nextViewsCountCollectDate;
        }

    }
}
