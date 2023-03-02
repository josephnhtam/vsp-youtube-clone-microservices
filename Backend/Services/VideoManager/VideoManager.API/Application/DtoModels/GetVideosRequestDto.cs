namespace VideoManager.API.Application.DtoModels {
    public class GetVideosRequestDto {
        public const int MaxPageSize = 50;

        private int _pageSize = 30;

        public int Page { get; set; } = 1;
        public int PageSize {
            get => _pageSize;
            set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
        }
        public string? Sort { get; set; }
    }
}
