namespace Search.Domain.Models {
    public class SearchableItem : EmptyItem {
        public UserProfile CreatorProfile { get; set; }
        public string Title { get; set; }
        public string[]? Contents { get; set; }
        public string[]? Tags { get; set; }
    }

    public class EmptyItem {
        public string Id { get; set; }
        public long Version { get; set; }
        public bool IsDeleted { get; set; }
    }
}
