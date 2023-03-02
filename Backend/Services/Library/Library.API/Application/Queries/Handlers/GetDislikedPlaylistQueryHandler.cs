using Application.Handlers;
using Library.API.Application.DtoModels;
using Library.API.Application.Queries.Handlers.Services;
using Library.Domain.Contracts;
using Library.Domain.Models;
using SharedKernel.Exceptions;

namespace Library.API.Application.Queries.Handlers {
    public class GetDislikedPlaylistQueryHandler : IQueryHandler<GetDislikedPlaylistQuery, PlaylistDto> {

        private readonly IUniquePlaylistRepository<DislikedPlaylist, PlaylistItem> _repository;
        private readonly IDtoResolver _dtoResolver;

        public GetDislikedPlaylistQueryHandler (IUniquePlaylistRepository<DislikedPlaylist, PlaylistItem> repository, IDtoResolver helper) {
            _repository = repository;
            _dtoResolver = helper;
        }

        public async Task<PlaylistDto> Handle (GetDislikedPlaylistQuery request, CancellationToken cancellationToken) {
            DislikedPlaylist? playlist = null;

            int? startPosition = null;
            if (request.IndexPagination != null) {
                var indexPagination = request.IndexPagination;

                if (indexPagination.Index.HasValue) {
                    startPosition = CalculateStartPosition(indexPagination.Index.Value, indexPagination.PageSize);

                    playlist = await _repository
                        .GetPlaylist(request.UserId, startPosition.Value, indexPagination.PageSize, cancellationToken);
                }

                if (indexPagination.VideoId.HasValue) {
                    if (!indexPagination.Index.HasValue || playlist?.Items.FirstOrDefault(x => x.VideoId == request.IndexPagination.VideoId) == null) {
                        var position = await _repository.GetPlaylistItemPosition(request.UserId, indexPagination.VideoId.Value, cancellationToken);

                        if (position >= 0) {
                            startPosition = CalculateStartPosition(position, indexPagination.PageSize);
                            playlist = await _repository
                                .GetPlaylist(request.UserId, startPosition.Value, indexPagination.PageSize, cancellationToken);
                        }
                    }
                }
            } else {
                playlist = await _repository.GetPlaylist(request.UserId, request.Pagination, cancellationToken);
            }

            if (playlist == null) {
                throw new AppException("Playlist not found", null, StatusCodes.Status404NotFound);
            }

            return await _dtoResolver.ResolvePlaylistDto<DislikedPlaylist, PlaylistItem>(playlist, request.UserId, startPosition, cancellationToken);
        }

        private int CalculateStartPosition (int index, int pageSize) {
            return (int)Math.Ceiling(Math.Max(0, index - pageSize * 0.5f));
        }
    }
}
