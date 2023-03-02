using Domain.Events;

namespace History.Domain.DomainEvents.Videos {
    public class VideoViewsMetricsChangedDomainEvent : IDomainEvent {
        public Guid VideoId { get; set; }
        public long ViewsCountChange { get; set; }

        public VideoViewsMetricsChangedDomainEvent (Guid videoId, long viewsCountChange) {
            VideoId = videoId;
            ViewsCountChange = viewsCountChange;
        }
    }
}
