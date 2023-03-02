using Application.Contracts;
using Library.API.Application.DtoModels;

namespace Library.API.Application.Queries {
    public class GetLibraryResourcesQuery : IQuery<GetLibraryResourcesResponseDto> {
        public string TargetUserId { get; set; }
        public bool RequireUploadedVideos { get; set; }
        public bool RequireCreatedPlaylistInfos { get; set; }
        public List<Guid>? RequireVideos { get; set; }
        public List<Guid>? RequirePlaylists { get; set; }
        public List<Guid>? RequirePlaylistInfos { get; set; }

        public int? MaxUploadedVideosCount { get; set; }
        public int? MaxCreatedPlaylistsCount { get; set; }
        public int? MaxPlaylistItemsCount { get; set; }

        public GetLibraryResourcesQuery (
            string targetUserId,
            bool requireUploadedVideos,
            bool requireCreatedPlaylistInfos,
            List<Guid>? requireVideos,
            List<Guid>? requirePlaylists,
            List<Guid>? requirePlaylistInfos,
            int? maxUploadedVideosCount,
            int? maxCreatedPlaylistsCount,
            int? maxPlaylistItemsCount) {
            TargetUserId = targetUserId;
            RequireUploadedVideos = requireUploadedVideos;
            RequireCreatedPlaylistInfos = requireCreatedPlaylistInfos;
            RequireVideos = requireVideos;
            RequirePlaylists = requirePlaylists;
            RequirePlaylistInfos = requirePlaylistInfos;
            MaxUploadedVideosCount = maxUploadedVideosCount;
            MaxCreatedPlaylistsCount = maxCreatedPlaylistsCount;
            MaxPlaylistItemsCount = maxPlaylistItemsCount;
        }

    }
}
