using Application.Contracts;

namespace Search.API.Application.Commands {
    public class UpdateVideoSearchInfoViewsMetricsCommand : ICommand {
        public Guid VideoId { get; set; }
        public long ViewsCount { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public UpdateVideoSearchInfoViewsMetricsCommand (Guid videoId, long viewsCount, DateTimeOffset updateDate) {
            VideoId = videoId;
            ViewsCount = viewsCount;
            UpdateDate = updateDate;
        }
    }
}
