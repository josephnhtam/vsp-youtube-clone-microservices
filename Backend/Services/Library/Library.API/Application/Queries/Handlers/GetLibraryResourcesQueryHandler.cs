using Application.Handlers;
using Library.API.Application.Configurations;
using Library.API.Application.DtoModels;
using Library.API.Application.Queries.Handlers.Services;
using Library.Domain.Contracts;
using Library.Domain.Models;
using Library.Domain.Specifications;
using Library.Infrastructure.Contracts;
using Microsoft.Extensions.Options;

namespace Library.API.Application.Queries.Handlers {
    public class GetLibraryResourcesQueryHandler : IQueryHandler<GetLibraryResourcesQuery, GetLibraryResourcesResponseDto> {

        private readonly ICachedVideoRepository _cachedVideoRepository;
        private readonly IPlaylistRepository<Playlist, OrderedPlaylistItem> _playlistRepository;
        private readonly IVideoQueryHelper _videoQueryHelper;
        private readonly IPlaylistQueryHelper _playlistQueryHelper;
        private readonly PlaylistQueryConfiguration _config;
        private readonly IDtoResolver _dtoResolver;

        public GetLibraryResourcesQueryHandler (
            ICachedVideoRepository cachedVideoRepository,
            IPlaylistRepository<Playlist, OrderedPlaylistItem> playlistRepository,
            IVideoQueryHelper videoQueryHelper,
            IPlaylistQueryHelper playlistQueryHelper,
            IOptions<PlaylistQueryConfiguration> config,
            IDtoResolver dtoResolver) {
            _cachedVideoRepository = cachedVideoRepository;
            _playlistRepository = playlistRepository;
            _videoQueryHelper = videoQueryHelper;
            _playlistQueryHelper = playlistQueryHelper;
            _config = config.Value;
            _dtoResolver = dtoResolver;
        }

        public async Task<GetLibraryResourcesResponseDto> Handle (GetLibraryResourcesQuery request, CancellationToken cancellationToken) {
            Resources resources = await GetResources(request, cancellationToken);
            return await ConvertToDtos(resources, cancellationToken);
        }

        private async Task<Resources> GetResources (GetLibraryResourcesQuery request, CancellationToken cancellationToken) {
            var getResourcesTasks = new GetResourcesTasks();

            if (request.RequireUploadedVideos) {
                getResourcesTasks.uploadedVideos = _videoQueryHelper.GetVideosAsync(
                    request.TargetUserId,
                    true,
                    VideoSort.CreateDate,
                    0,
                    request.MaxUploadedVideosCount ?? 12,
                    cancellationToken);
            }

            if (request.RequireCreatedPlaylistInfos) {
                getResourcesTasks.createdPLaylists = _playlistQueryHelper.GetPlaylistsAsync(
                    request.TargetUserId,
                    true,
                    request.MaxCreatedPlaylistsCount ?? 12,
                    null,
                    cancellationToken);
            }

            if (request.RequireVideos?.Count > 0) {
                getResourcesTasks.videos = _cachedVideoRepository.GetVideosAsync(request.RequireVideos, cancellationToken);
            }

            if (request.RequirePlaylists?.Count > 0) {
                getResourcesTasks.playlists = _playlistRepository.GetPlaylists(
                    request.RequirePlaylists,
                    request.MaxPlaylistItemsCount ?? 12,
                    true,
                    cancellationToken);
            }

            if (request.RequirePlaylistInfos?.Count > 0) {
                getResourcesTasks.playlistsForInfos = _playlistRepository.GetPlaylists(
                    request.RequirePlaylistInfos,
                    _config.NonAvailableVideosTolerance + 1,
                    true,
                    cancellationToken);
            }

            await Task.WhenAll(getResourcesTasks.GetTasks());

            return new Resources {
                uploadedVideos = getResourcesTasks.uploadedVideos?.Result,
                createdPlaylists = getResourcesTasks.createdPLaylists?.Result,
                videos = getResourcesTasks.videos?.Result,
                playlists = getResourcesTasks.playlists?.Result,
                playlistsForInfos = getResourcesTasks.playlistsForInfos?.Result,
            };
        }

        private async Task<GetLibraryResourcesResponseDto> ConvertToDtos (Resources resources, CancellationToken cancellationToken) {
            var conversionsTasks = new DtoConversionsTasks();

            if (resources.uploadedVideos != null) {
                conversionsTasks.uploadedVideos =
                    _dtoResolver.ResolveVideoDtos(resources.uploadedVideos.Videos, cancellationToken);
            }

            if (resources.videos != null) {
                conversionsTasks.videos =
                    _dtoResolver.ResolveVideoDtos(resources.videos, cancellationToken);
            }

            if (resources.createdPlaylists != null) {
                conversionsTasks.createdPlaylistInfos =
                    _dtoResolver.ResolvePlaylistInfoDtos(resources.createdPlaylists.Playlists, null, cancellationToken);
            }

            if (resources.playlists != null) {
                conversionsTasks.playlists =
                    _dtoResolver.ResolvePlaylistDtos(resources.playlists, null, cancellationToken);
            }

            if (resources.playlistsForInfos != null) {
                conversionsTasks.playlistInfos =
                    _dtoResolver.ResolvePlaylistInfoDtos(resources.playlistsForInfos, null, cancellationToken);
            }

            await Task.WhenAll(conversionsTasks.GetTasks());

            RemoveHiddenVideoItem(conversionsTasks.playlists?.Result);

            return new GetLibraryResourcesResponseDto {
                UploadedVideos = conversionsTasks.uploadedVideos?.Result,
                CreatedPlaylistInfos = conversionsTasks.createdPlaylistInfos?.Result,
                Videos = conversionsTasks.videos?.Result,
                Playlists = conversionsTasks.playlists?.Result,
                PlaylistInfos = conversionsTasks.playlistInfos?.Result,
            };
        }

        private void RemoveHiddenVideoItem (List<PlaylistDto>? playlists) {
            if (playlists != null) {
                foreach (var playlist in playlists) {
                    playlist.Items = playlist.Items.Where(x => x.Video is not HiddenVideoDto).ToList();
                }
            }
        }

        private class GetResourcesTasks {
            public Task<GetVideosResult>? uploadedVideos = null;
            public Task<GetPlaylistsResult>? createdPLaylists = null;
            public Task<List<Video>>? videos = null;
            public Task<List<Playlist>>? playlists = null;
            public Task<List<Playlist>>? playlistsForInfos = null;

            private IEnumerable<Task?> GetNullableTasks () {
                yield return uploadedVideos;
                yield return createdPLaylists;
                yield return videos;
                yield return playlists;
                yield return playlistsForInfos;
            }

            public IEnumerable<Task> GetTasks () {
                return GetNullableTasks().Where(x => x != null)!;
            }
        }

        private class Resources {
            public GetVideosResult? uploadedVideos = null;
            public GetPlaylistsResult? createdPlaylists = null;
            public List<Video>? videos = null;
            public List<Playlist>? playlists = null;
            public List<Playlist>? playlistsForInfos = null;
        }

        private class DtoConversionsTasks {
            public Task<List<VideoDto>>? uploadedVideos = null;
            public Task<List<PlaylistInfoDto>>? createdPlaylistInfos = null;
            public Task<List<VideoDto>>? videos = null;
            public Task<List<PlaylistDto>>? playlists = null;
            public Task<List<PlaylistInfoDto>>? playlistInfos = null;

            private IEnumerable<Task?> GetNullableTasks () {
                yield return uploadedVideos;
                yield return createdPlaylistInfos;
                yield return videos;
                yield return playlists;
                yield return playlistInfos;
            }

            public IEnumerable<Task> GetTasks () {
                return GetNullableTasks().Where(x => x != null)!;
            }
        }

    }
}
