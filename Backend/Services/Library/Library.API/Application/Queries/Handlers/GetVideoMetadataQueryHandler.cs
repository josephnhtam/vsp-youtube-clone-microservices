using Application.Handlers;
using Library.API.Application.DtoModels;
using Library.Domain.Contracts;
using Library.Domain.Models;

namespace Library.API.Application.Queries.Handlers {
    public class GetVideoMetadataQueryHandler : IQueryHandler<GetVideoMetadataQuery, VideoMetadataDto?> {

        private readonly IVideoRepository _videoRepository;
        private readonly IUniquePlaylistRepository<LikedPlaylist, PlaylistItem> _likedPlaylistRepository;
        private readonly IUniquePlaylistRepository<DislikedPlaylist, PlaylistItem> _dislikedPlaylistRepository;

        public GetVideoMetadataQueryHandler (
            IVideoRepository videoRepository,
            IUniquePlaylistRepository<LikedPlaylist, PlaylistItem> likedPlaylistRepository,
            IUniquePlaylistRepository<DislikedPlaylist, PlaylistItem> dislikedPlaylistRepository) {
            _videoRepository = videoRepository;
            _likedPlaylistRepository = likedPlaylistRepository;
            _dislikedPlaylistRepository = dislikedPlaylistRepository;
        }

        public async Task<VideoMetadataDto?> Handle (GetVideoMetadataQuery request, CancellationToken cancellationToken) {
            var video = await _videoRepository.GetVideoByIdAsync(request.VideoId, false, cancellationToken);
            if (video == null) return null;

            if (request.UserId == null) {
                return new VideoMetadataDto {
                    ViewsCount = video.Metrics.ViewsCount,
                    LikesCount = video.Metrics.LikesCount,
                    DislikesCount = video.Metrics.DislikesCount,
                    UserVote = VoteType.None,
                };
            } else {
                var getIsLikedTask = _likedPlaylistRepository.IsInPlaylist(request.UserId, video.Id, cancellationToken);
                var getIsDislikedTask = _dislikedPlaylistRepository.IsInPlaylist(request.UserId, video.Id, cancellationToken);

                await Task.WhenAll(getIsLikedTask, getIsDislikedTask);

                var isLiked = getIsLikedTask.Result;
                var isDisliked = getIsDislikedTask.Result;

                return new VideoMetadataDto {
                    ViewsCount = video.Metrics.ViewsCount,
                    LikesCount = video.Metrics.LikesCount,
                    DislikesCount = video.Metrics.DislikesCount,
                    UserVote = isLiked ? VoteType.Like : isDisliked ? VoteType.Dislike : VoteType.None,
                };
            }
        }

    }
}
