namespace Search.API.Application.DtoModels {
    public class SearchableItemDto {
        public string Id { get; set; }
        public string Title { get; set; }
        public CreatorProfileDto CreatorProfile { get; set; }
    }
}
