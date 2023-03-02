
using VideoManager.Domain.Specifications;

namespace VideoManager.API.Application.Utilities {
    public static class VideoSorts {
        public const string DateDesc = "date-desc";
        public const string DateAsc = "date-asc";
        public const string ViewsDesc = "views-desc";
        public const string ViewsAsc = "views-asc";

        public static VideoSort ToVideoSort (this string? text) {
            switch (text?.ToLower()) {
                default:
                case DateDesc:
                    return VideoSort.DateDesc;
                case DateAsc:
                    return VideoSort.DateAsc;
                case ViewsDesc:
                    return VideoSort.ViewsDesc;
                case ViewsAsc:
                    return VideoSort.ViewsAsc;
            }
        }
    }
}
