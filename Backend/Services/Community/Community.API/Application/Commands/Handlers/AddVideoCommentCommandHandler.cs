using Application.Handlers;
using Community.Domain.Contracts;
using Community.Domain.Models;
using Domain.Contracts;
using SharedKernel.Exceptions;

namespace Community.API.Application.Commands.Handlers {
    public class AddVideoCommentCommandHandler : ICommandHandler<AddVideoCommentCommand, VideoComment> {

        private readonly IVideoForumRepository _forumRepository;
        private readonly IVideoCommentRepository _commentRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddVideoCommentCommandHandler> _logger;

        public AddVideoCommentCommandHandler (
            IVideoForumRepository forumRepository,
            IVideoCommentRepository commentRepository,
            IUserProfileRepository userProfileRepository,
            IUnitOfWork unitOfWork,
            ILogger<AddVideoCommentCommandHandler> logger) {
            _forumRepository = forumRepository;
            _commentRepository = commentRepository;
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<VideoComment> Handle (AddVideoCommentCommand request, CancellationToken cancellationToken) {
            if (request.ParentCommentId != null) {
                return await AddReplyToCommentAsync(request);
            } else if (request.VideoId != null) {
                return await AddCommentToForumAsync(request);
            }

            throw new AppException(null, null, StatusCodes.Status400BadRequest);
        }

        private async Task CheckUserProfile (AddVideoCommentCommand request) {
            var userProfile = await _userProfileRepository.GetUserProfileAsync(request.UserId);

            if (userProfile == null) {
                throw new AppException("User profile not found", null, StatusCodes.Status404NotFound);
            }
        }

        private async Task<VideoComment> AddCommentToForumAsync (AddVideoCommentCommand request) {
            VideoComment comment = null!;

            await _unitOfWork.ExecuteTransactionAsync(async () => {
                await CheckUserProfile(request);

                var forum = await _forumRepository.GetVideoForumByIdAsync(request.VideoId!.Value);

                if (forum == null) {
                    throw new AppException("The forum does not exist.", null, StatusCodes.Status400BadRequest);
                }

                if (!forum.AllowedToComment) {
                    throw new AppException("The forum does not allow to comment.", null, StatusCodes.Status400BadRequest);
                }

                comment = forum.AddComment(request.UserId, request.Comment);

                await _commentRepository.AddVideoCommentAsync(comment);
                await _unitOfWork.CommitAsync();
            });

            return comment!;
        }

        private async Task<VideoComment> AddReplyToCommentAsync (AddVideoCommentCommand request) {
            VideoComment comment = null!;

            await _unitOfWork.ExecuteTransactionAsync(async () => {
                await CheckUserProfile(request);

                var commentToReply = await _commentRepository.GetVideoCommentByIdAsync(request.ParentCommentId!.Value, true);

                if (commentToReply == null) {
                    throw new AppException("The comment to reply does not exist.", null, StatusCodes.Status400BadRequest);
                }

                comment = commentToReply.Reply(request.UserId, request.Comment);

                await _commentRepository.AddVideoCommentAsync(comment);
                await _unitOfWork.CommitAsync();
            });

            return comment;
        }
    }
}
