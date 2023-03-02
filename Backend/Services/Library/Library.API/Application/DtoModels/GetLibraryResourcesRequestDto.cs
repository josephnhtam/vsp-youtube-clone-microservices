namespace Library.API.Application.DtoModels {
    public class GetLibraryResourcesRequestDto {
        public string TargetUserId { get; set; }
        public bool RequireUploadedVideos { get; set; }
        public bool RequireCreatedPlaylistInfos { get; set; }
        public List<Guid>? RequireVideos { get; set; }
        public List<Guid>? RequirePlaylists { get; set; }
        public List<Guid>? RequirePlaylistInfos { get; set; }

        public int? MaxUploadedVideosCount { get; set; }
        public int? MaxCreatedPlaylistsCount { get; set; }
        public int? MaxPlaylistItemsCount { get; set; }
    }
}
