namespace Search.Infrastructure.Specifications {
    public class VideoSearchParameters {
        public string? Query { get; init; }
        public Pagination Pagination { get; init; }
        public PeriodRange? PeriodRange { get; init; }
        public IEnumerable<string>? CreatorIds { get; init; }
        public VideoSort Sort { get; init; }
    }
}
