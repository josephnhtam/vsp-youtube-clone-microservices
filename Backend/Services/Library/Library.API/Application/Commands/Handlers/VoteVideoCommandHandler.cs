using Application.Handlers;
using Domain.Contracts;
using Library.API.Application.Configurations;
using Library.Domain.Contracts;
using Library.Domain.Models;
using MediatR;
using Microsoft.Extensions.Options;
using SharedKernel.Exceptions;

namespace Library.API.Application.Commands.Handlers {
    public class VoteVideoCommandHandler : ICommandHandler<VoteVideoCommand> {

        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IUniquePlaylistRepository<LikedPlaylist, PlaylistItem> _likedPlaylistRepository;
        private readonly IUniquePlaylistRepository<DislikedPlaylist, PlaylistItem> _dislikedPlaylistRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MetricsSyncConfiguration _config;

        public VoteVideoCommandHandler (
            IUserProfileRepository userProfileRepository,
            IVideoRepository videoRepository,
            IUniquePlaylistRepository<LikedPlaylist, PlaylistItem> likedPlaylistRepository,
            IUniquePlaylistRepository<DislikedPlaylist, PlaylistItem> dislikedPlaylistRepository,
            IUnitOfWork unitOfWork,
            IOptions<MetricsSyncConfiguration> config) {
            _userProfileRepository = userProfileRepository;
            _videoRepository = videoRepository;
            _likedPlaylistRepository = likedPlaylistRepository;
            _dislikedPlaylistRepository = dislikedPlaylistRepository;
            _unitOfWork = unitOfWork;
            _config = config.Value;
        }

        public async Task<Unit> Handle (VoteVideoCommand request, CancellationToken cancellationToken) {
            var getUserProfileTask = _userProfileRepository.GetUserProfileAsync(request.UserId, false, cancellationToken);
            var getVideoTask = _videoRepository.GetVideoByIdAsync(request.VideoId, false, cancellationToken);

            await Task.WhenAll(getUserProfileTask, getVideoTask);

            var video = getVideoTask.Result;
            var userProfile = getUserProfileTask.Result;

            if (video == null && request.VoteType != VoteType.None) {
                throw new AppException($"Video {request.VideoId} not found", null, StatusCodes.Status404NotFound);
            }

            if (userProfile == null) {
                throw new AppException("User profile is not reigstered", null, StatusCodes.Status500InternalServerError);
            }

            await _unitOfWork.ExecuteTransactionAsync(async () => {
                var likedPlaylist = await _likedPlaylistRepository.GetPlaylistIncludingVideo(request.UserId, request.VideoId, true, cancellationToken);
                var dislikedPlaylist = await _dislikedPlaylistRepository.GetPlaylistIncludingVideo(request.UserId, request.VideoId, true, cancellationToken);

                if (likedPlaylist == null || dislikedPlaylist == null) {
                    throw new AppException(null, null, StatusCodes.Status500InternalServerError);
                }

                int likedPlaylistCountChange = 0;
                int dislikedPlaylistCountChange = 0;

                switch (request.VoteType) {
                    case VoteType.None:
                        likedPlaylistCountChange = likedPlaylist.RemoveVideo(request.VideoId) ? -1 : 0;
                        dislikedPlaylistCountChange = dislikedPlaylist.RemoveVideo(request.VideoId) ? -1 : 0;
                        break;
                    case VoteType.Like:
                        likedPlaylistCountChange = likedPlaylist.AddVideo(request.VideoId, true) ? 1 : 0;
                        dislikedPlaylistCountChange = dislikedPlaylist.RemoveVideo(request.VideoId) ? -1 : 0;
                        break;
                    case VoteType.Dislike:
                        likedPlaylistCountChange = likedPlaylist.RemoveVideo(request.VideoId) ? -1 : 0;
                        dislikedPlaylistCountChange = dislikedPlaylist.AddVideo(request.VideoId, true) ? 1 : 0;
                        break;
                }

                if (video != null) {
                    if (likedPlaylistCountChange != 0 || dislikedPlaylistCountChange != 0) {
                        video.ChangeVoteMetrics(likedPlaylistCountChange, dislikedPlaylistCountChange, _config.SyncDelay);
                    }
                }

                await _unitOfWork.CommitAsync(cancellationToken);
            });

            return Unit.Value;
        }

    }
}
