namespace History.Infrastructure.Specifications {
    public class UserWatchHistorySearchParameters {
        public string UserId { get; set; }
        public string? Query { get; init; }
        public Pagination Pagination { get; init; }
        public PeriodRange? PeriodRange { get; init; }
    }
}
