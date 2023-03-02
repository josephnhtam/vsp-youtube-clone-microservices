namespace VideoManager.API.Application.DtoModels {
    public class GetVideosResponseDto {
        public List<VideoDto> Videos { get; set; }
        public int TotalCount { get; set; }
    }
}
