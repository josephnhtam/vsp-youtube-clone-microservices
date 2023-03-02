using Domain;

namespace History.Domain.Models {
    public class VideoMetrics : ValueObject {

        public long ViewsCount { get; internal set; }

        public DateTimeOffset? NextSyncDate { get; internal set; }

        private VideoMetrics () {
            ViewsCount = 0;
        }

        public static VideoMetrics Create () {
            return new VideoMetrics();
        }

    }
}
