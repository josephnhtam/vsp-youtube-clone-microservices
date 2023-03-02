using Application.Contracts;

namespace VideoManager.API.Application.Commands {
    public class UpdateVideoCommentsMetricsCommand : ICommand {
        public Guid VideoId { get; set; }
        public long CommentsCount { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public UpdateVideoCommentsMetricsCommand (Guid videoId, long commentsCount, DateTimeOffset updateDate) {
            VideoId = videoId;
            CommentsCount = commentsCount;
            UpdateDate = updateDate;
        }
    }
}
