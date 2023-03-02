
namespace Library.API.Application.DtoModels {
    public class GetVideosResponseDto {
        public int TotalCount { get; set; }
        public List<VideoDto> Videos { get; set; }
    }
}
