namespace Library.API.Application.DtoModels {
    public class GetLibraryResourcesResponseDto {
        public List<VideoDto>? UploadedVideos { get; set; }
        public List<PlaylistInfoDto>? CreatedPlaylistInfos { get; set; }
        public List<VideoDto>? Videos { get; set; }
        public List<PlaylistDto>? Playlists { get; set; }
        public List<PlaylistInfoDto>? PlaylistInfos { get; set; }
    }
}
