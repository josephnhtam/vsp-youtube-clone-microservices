using Application.Handlers;
using AutoMapper;
using Library.API.Application.DtoModels;
using Library.Domain.Contracts;
using Library.Domain.Models;

namespace Library.API.Application.Queries.Handlers {
    public class GetPlaylistsWithVideoQueryHandler : IQueryHandler<GetPlaylistsWithVideoQuery, PlaylistsWithVideoDto> {

        private readonly IUniquePlaylistRepository<WatchLaterPlaylist, OrderedPlaylistItem> _watchLaterPlaylistRepository;
        private readonly IPlaylistRepository<Playlist, OrderedPlaylistItem> _playlistRepository;
        private readonly IMapper _mapper;

        public GetPlaylistsWithVideoQueryHandler (
            IUniquePlaylistRepository<WatchLaterPlaylist, OrderedPlaylistItem> watchLaterPlaylistRepository,
            IPlaylistRepository<Playlist, OrderedPlaylistItem> playlistRepository,
            IMapper mapper) {
            _watchLaterPlaylistRepository = watchLaterPlaylistRepository;
            _playlistRepository = playlistRepository;
            _mapper = mapper;
        }

        public async Task<PlaylistsWithVideoDto> Handle (GetPlaylistsWithVideoQuery request, CancellationToken cancellationToken) {
            var getIsVideoInWatchLaterPlaylistTask = _watchLaterPlaylistRepository.IsInPlaylist(request.UserId, request.VideoId, cancellationToken);
            var getPlaylistsTask = _playlistRepository.GetPlaylistsIncludingVideo(request.UserId, request.VideoId, cancellationToken);

            await Task.WhenAll(getIsVideoInWatchLaterPlaylistTask, getPlaylistsTask);

            return new PlaylistsWithVideoDto {
                IsAddedToWatchLaterPlaylist = getIsVideoInWatchLaterPlaylistTask.Result,
                PlaylistsWithVideo = _mapper.Map<List<SimplePlaylistDto>>(
                    getPlaylistsTask.Result.Where(x => x.Items.Any())),
                PlaylistsWithoutVideo = _mapper.Map<List<SimplePlaylistDto>>(
                    getPlaylistsTask.Result.Where(x => !x.Items.Any())),
            };
        }
    }
}
