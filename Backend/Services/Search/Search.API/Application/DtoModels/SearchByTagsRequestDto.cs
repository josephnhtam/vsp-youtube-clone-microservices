namespace Search.API.Application.DtoModels {
    public class SearchByTagsRequestDto {
        public List<string> Tags { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
