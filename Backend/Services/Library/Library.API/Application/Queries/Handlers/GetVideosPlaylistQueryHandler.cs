using Application.Handlers;
using Library.API.Application.DtoModels;
using Library.API.Application.Queries.Handlers.Services;
using Library.Domain.Specifications;
using Library.Infrastructure.Contracts;
using SharedKernel.Exceptions;

namespace Library.API.Application.Queries.Handlers {
    public class GetVideosPlaylistQueryHandler : IQueryHandler<GetVideosPlaylistQuery, PlaylistDto> {

        private readonly IVideoQueryHelper _queryHelper;
        private readonly IDtoResolver _dtoResolver;

        public GetVideosPlaylistQueryHandler (IVideoQueryHelper queryHelper, IDtoResolver dtoResolver) {
            _queryHelper = queryHelper;
            _dtoResolver = dtoResolver;
        }

        public async Task<PlaylistDto> Handle (GetVideosPlaylistQuery request, CancellationToken cancellationToken) {
            GetVideosResult? videos = null;

            int? startPosition = null;
            if (request.IndexPagination != null) {
                var indexPagination = request.IndexPagination;

                if (indexPagination.Index.HasValue) {
                    startPosition = CalculateStartPosition(indexPagination.Index.Value, indexPagination.PageSize);

                    videos = await _queryHelper
                        .GetVideosAsync(request.VideosUserId, true, VideoSort.CreateDate, startPosition.Value, indexPagination.PageSize, cancellationToken);
                }

                if (indexPagination.VideoId.HasValue) {
                    if (!indexPagination.Index.HasValue || videos?.Videos.FirstOrDefault(x => x.Id == request.IndexPagination.VideoId) == null) {
                        var index = await _queryHelper.GetVideoIndex(request.VideosUserId, true, indexPagination.VideoId.Value, VideoSort.CreateDate, cancellationToken);

                        if (index >= 0) {
                            startPosition = CalculateStartPosition(index, indexPagination.PageSize);

                            videos = await _queryHelper
                                .GetVideosAsync(request.VideosUserId, true, VideoSort.CreateDate, startPosition.Value, indexPagination.PageSize, cancellationToken);
                        }
                    }
                }
            } else {
                if (request.Pagination != null) {
                    var skip = Math.Max(0, (request.Pagination.Page - 1) * request.Pagination.PageSize);
                    var limit = request.Pagination.PageSize;

                    videos = await _queryHelper.GetVideosAsync(request.VideosUserId, true, VideoSort.CreateDate, skip, limit, cancellationToken);
                } else {
                    videos = await _queryHelper.GetVideosAsync(request.VideosUserId, true, VideoSort.CreateDate, null, null, cancellationToken);
                }
            }

            if (videos == null || videos.TotalCount == 0) {
                throw new AppException("Playlist not found", null, StatusCodes.Status404NotFound);
            }

            string playlistId = $"videos-{request.VideosUserId}";
            var playlistDto = await _dtoResolver.ResolvePlaylistDtoFromVideos(videos, playlistId, request.UserId, startPosition, cancellationToken);
            return playlistDto;
        }

        private int CalculateStartPosition (int index, int pageSize) {
            return (int)Math.Ceiling(Math.Max(0, index - pageSize * 0.5f));
        }

    }
}
