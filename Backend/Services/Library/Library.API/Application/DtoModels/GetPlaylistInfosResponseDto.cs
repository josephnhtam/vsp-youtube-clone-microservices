namespace Library.API.Application.DtoModels {
    public class GetPlaylistInfosResponseDto {
        public int TotalCount { get; set; }
        public List<PlaylistInfoDto> Infos { get; set; }
    }
}
