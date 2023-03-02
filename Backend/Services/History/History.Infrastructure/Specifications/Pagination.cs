namespace History.Infrastructure.Specifications {
    public class Pagination {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        public Pagination () { }

        public Pagination (int page, int pageSize) {
            Page = page;
            PageSize = pageSize;
        }
    }
}
