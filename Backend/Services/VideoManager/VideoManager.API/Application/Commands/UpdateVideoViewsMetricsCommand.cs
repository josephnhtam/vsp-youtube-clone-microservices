using Application.Contracts;

namespace VideoManager.API.Application.Commands {
    public class UpdateVideoViewsMetricsCommand : ICommand {
        public Guid VideoId { get; set; }
        public long ViewsCount { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public UpdateVideoViewsMetricsCommand (Guid videoId, long viewsCount, DateTimeOffset updateDate) {
            VideoId = videoId;
            ViewsCount = viewsCount;
            UpdateDate = updateDate;
        }
    }
}
