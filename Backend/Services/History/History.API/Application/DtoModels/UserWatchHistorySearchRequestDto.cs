namespace History.API.Application.DtoModels {
    public class UserWatchHistorySearchRequestDto {
        public string? Query { get; set; }
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
