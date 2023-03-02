namespace Search.API.Application.DtoModels {
    public class SearchByQueryRequestDto {
        public SearchTarget SearchTarget { get; set; }
        public string Query { get; set; }
        public SearchSort? Sort { get; set; }
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
