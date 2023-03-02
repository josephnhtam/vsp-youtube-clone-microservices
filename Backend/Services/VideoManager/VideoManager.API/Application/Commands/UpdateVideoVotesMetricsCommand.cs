using Application.Contracts;

namespace VideoManager.API.Application.Commands {
    public class UpdateVideoVotesMetricsCommand : ICommand {
        public Guid VideoId { get; set; }
        public long LikesCount { get; set; }
        public long DislikesCount { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public UpdateVideoVotesMetricsCommand (Guid videoId, long likesCount, long dislikesCount, DateTimeOffset updateDate) {
            VideoId = videoId;
            LikesCount = likesCount;
            DislikesCount = dislikesCount;
            UpdateDate = updateDate;
        }
    }
}
