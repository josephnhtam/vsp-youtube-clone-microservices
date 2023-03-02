using Domain.Events;

namespace Library.Domain.DomainEvents.Videos {
    public class VideoViewsMetricsUpdatedDomainEvent : IDomainEvent {
        public Guid VideoId { get; set; }
        public long ViewsCount { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        public VideoViewsMetricsUpdatedDomainEvent (Guid videoId, long viewsCount, DateTimeOffset? updateDate) {
            VideoId = videoId;
            ViewsCount = viewsCount;
            UpdateDate = updateDate;
        }
    }
}
