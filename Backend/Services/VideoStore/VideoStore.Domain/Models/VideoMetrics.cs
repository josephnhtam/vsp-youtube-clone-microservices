using Domain;

namespace VideoStore.Domain.Models {
    public class VideoMetrics : ValueObject {

        public long ViewsCount { get; internal set; }

        public DateTimeOffset? ViewsCountUpdateDate { get; internal set; }

        private VideoMetrics () {
            ViewsCount = 0;
        }

        public static VideoMetrics Create () {
            return new VideoMetrics();
        }

    }
}
