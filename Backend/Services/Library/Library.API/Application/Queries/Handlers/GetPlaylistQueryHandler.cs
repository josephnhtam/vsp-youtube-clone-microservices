using Application.Handlers;
using Library.API.Application.DtoModels;
using Library.API.Application.Queries.Handlers.Services;
using Library.Domain.Contracts;
using Library.Domain.Models;
using SharedKernel.Exceptions;

namespace Library.API.Application.Queries.Handlers {
    public class GetPlaylistQueryHandler : IQueryHandler<GetPlaylistQuery, PlaylistDto> {

        private readonly IPlaylistRepository<Playlist, OrderedPlaylistItem> _repository;
        private readonly IDtoResolver _dtoResolver;

        public GetPlaylistQueryHandler (IPlaylistRepository<Playlist, OrderedPlaylistItem> repository, IDtoResolver dtoResolver) {
            _repository = repository;
            _dtoResolver = dtoResolver;
        }

        public async Task<PlaylistDto> Handle (GetPlaylistQuery request, CancellationToken cancellationToken) {
            Playlist? playlist = null;

            if (request.IndexPagination != null) {
                var indexPagination = request.IndexPagination;
                int? startPosition = null;

                if (indexPagination.Index.HasValue) {
                    startPosition = CalculateStartPosition(indexPagination.Index.Value, indexPagination.PageSize);

                    playlist = await _repository
                        .GetPlaylist(request.PlaylistId, startPosition.Value, indexPagination.PageSize, cancellationToken);
                }

                if (indexPagination.VideoId.HasValue) {
                    if (!indexPagination.Index.HasValue || playlist?.Items.FirstOrDefault(x => x.VideoId == request.IndexPagination.VideoId) == null) {
                        var position = await _repository.GetPlaylistItemPosition(request.PlaylistId, indexPagination.VideoId.Value, cancellationToken);

                        if (position >= 0) {
                            startPosition = CalculateStartPosition(position, indexPagination.PageSize); ;
                            playlist = await _repository
                                .GetPlaylist(request.PlaylistId, startPosition.Value, indexPagination.PageSize, cancellationToken);
                        }
                    }
                }
            } else {
                playlist = await _repository.GetPlaylist(request.PlaylistId, request.Pagination, cancellationToken);
            }

            if (playlist == null) {
                throw new AppException("Playlist not found", null, StatusCodes.Status404NotFound);
            }

            if (playlist.Visibility == PlaylistVisibility.Private && playlist.UserId != request.UserId) {
                throw new AppException("The playlist is private", null, StatusCodes.Status403Forbidden);
            }

            var playlistDto = await _dtoResolver.ResolvePlaylistDto<Playlist, OrderedPlaylistItem>(playlist, request.UserId, null, cancellationToken);

            playlistDto.Title = playlist.Title;
            playlistDto.Description = playlist.Description;
            playlistDto.Visibility = playlist.Visibility;

            return playlistDto;
        }

        private int CalculateStartPosition (int index, int pageSize) {
            return (int)Math.Ceiling(Math.Max(0, index - pageSize * 0.5f));
        }
    }
}
