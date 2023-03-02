using Domain;

namespace VideoManager.Domain.Models {
    public class VideoMetrics : ValueObject {

        public long ViewsCount { get; internal set; }
        public long CommentsCount { get; internal set; }
        public long LikesCount { get; internal set; }
        public long DislikesCount { get; internal set; }

        public DateTimeOffset? ViewsCountUpdateDate { get; internal set; }
        public DateTimeOffset? CommentsCountUpdateDate { get; internal set; }
        public DateTimeOffset? VotesCountUpdateDate { get; internal set; }

        private VideoMetrics () {
            ViewsCount = 0;
            CommentsCount = 0;
            LikesCount = 0;
            DislikesCount = 0;
        }

        public static VideoMetrics Create () {
            return new VideoMetrics();
        }

    }
}
