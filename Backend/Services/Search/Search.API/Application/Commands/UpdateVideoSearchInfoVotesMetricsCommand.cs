using Application.Contracts;

namespace Search.API.Application.Commands {
    public class UpdateVideoSearchInfoVotesMetricsCommand : ICommand {
        public Guid VideoId { get; set; }
        public long LikesCount { get; set; }
        public long DislikesCount { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public UpdateVideoSearchInfoVotesMetricsCommand (Guid videoId, long likesCount, long dislikesCount, DateTimeOffset updateDate) {
            VideoId = videoId;
            LikesCount = likesCount;
            DislikesCount = dislikesCount;
            UpdateDate = updateDate;
        }
    }
}
