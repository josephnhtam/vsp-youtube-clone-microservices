namespace Search.API.Application.DtoModels {
    public class SearchByCreatorIdsDto {
        public List<string> CreatorIds { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
