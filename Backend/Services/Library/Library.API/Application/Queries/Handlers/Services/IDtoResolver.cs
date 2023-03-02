using Library.API.Application.DtoModels;
using Library.Domain.Models;
using Library.Infrastructure.Contracts;

namespace Library.API.Application.Queries.Handlers.Services {
    public interface IDtoResolver {
        Task<List<VideoDto>> ResolveVideoDtos (List<Video> videos, CancellationToken cancellationToken = default);

        Task<PlaylistDto> ResolvePlaylistDto<TPlaylist, TPlaylistItem> (TPlaylist playlist, string? userId,
            int? itemStartPosition, CancellationToken cancellationToken = default)
            where TPlaylist : PlaylistBase<TPlaylistItem>
            where TPlaylistItem : PlaylistItem;

        Task<List<PlaylistDto>> ResolvePlaylistDtos (List<Playlist> playlists, string? userId, CancellationToken cancellationToken = default);

        Task<PlaylistDto> ResolvePlaylistDtoFromVideos (GetVideosResult videos, string playlistId, string? userId,
            int? itemStartPosition, CancellationToken cancellationToken = default);

        Task<List<PlaylistInfoDto>> ResolvePlaylistInfoDtos (List<Playlist> playlist, string? userId,
            CancellationToken cancellationToken = default);
    }
}
