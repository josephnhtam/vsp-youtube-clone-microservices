namespace Library.API.Application.DtoModels {
    public class GetSimplePlaylistInfosResponseDto {
        public int TotalCount { get; set; }
        public List<SimplePlaylistInfoDto> Infos { get; set; }
    }
}
