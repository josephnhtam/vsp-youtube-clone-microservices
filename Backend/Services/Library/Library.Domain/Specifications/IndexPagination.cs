namespace Library.Domain.Specifications {
    public class IndexPagination {
        public Guid? VideoId { get; set; }
        public int? Index { get; set; } = 0;
        public int PageSize { get; set; } = 50;

        public IndexPagination () { }

        public IndexPagination (Guid? videoId, int? index, int pageSize) {
            VideoId = videoId;
            Index = index;
            PageSize = pageSize;

            if (!videoId.HasValue && !index.HasValue) {
                index = 0;
            }
        }
    }
}
