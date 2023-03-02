namespace Search.API.Application.DtoModels {
    public class SearchResponseDto {
        public long TotalCount { get; set; }
        public List<object> Items { get; set; }
    }
}
