namespace Search.Infrastructure.Configurations {
    public class ScoreBoostConfiguration {
        public SearchVideoBoostConfiguration SearchVideoConfig { get; set; } = new SearchVideoBoostConfiguration();
        public SearchVideoByTagsBoostConfiguration SearchVideoByTagsConfig { get; set; } = new SearchVideoByTagsBoostConfiguration();
    }

    public class SearchVideoBoostConfiguration {
        public float TitleMatchBoost { get; set; } = 1f;
        public float CreatorNameMatchBoost { get; set; } = 0.3f;
        public float TagsMatchBoost { get; set; } = 0.2f;
        public TimeDistanceBoostConfiguration TimeDistasnceBoost { get; set; } = new TimeDistanceBoostConfiguration();
        public VideoMetricsBoostConfiguration MetricsBoost { get; set; } = new VideoMetricsBoostConfiguration();

    }

    public class SearchVideoByTagsBoostConfiguration {
        public float SearchByTagsBoost { get; set; } = 1f;
        public float TagsMatchBoost { get; set; } = 0.5f;
        public TimeDistanceBoostConfiguration TimeDistasnceBoost { get; set; } = new TimeDistanceBoostConfiguration();
        public VideoMetricsBoostConfiguration MetricsBoost { get; set; } = new VideoMetricsBoostConfiguration();
    }

    public class VideoMetricsBoostConfiguration {
        public float ViewsCountFactor { get; set; } = 1e-4f;
        public float LikesCountFactor { get; set; } = 3e-4f;
        public float DislikesCountFactor { get; set; } = -6e-4f;
        public float MaxMetricsBoost { get; set; } = 2f;
    }

    public class TimeDistanceBoostConfiguration {
        public float TimeDistanceBoost { get; set; } = 0.75f;
        public float TimePivotDays { get; set; } = 30f;
    }
}
