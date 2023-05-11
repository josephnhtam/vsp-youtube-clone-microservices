using Application.Handlers;
using Community.Domain.Contracts;
using Community.Domain.Models;
using Domain.Contracts;
using MediatR;
using SharedKernel.Exceptions;

namespace Community.API.Application.Commands.Handlers {
    public class VoteVideoCommentCommandHandler : ICommandHandler<VoteVideoCommentCommand> {

        private readonly IVideoCommentRepository _commentRepository;
        private readonly IVideoCommentVoteRepository _commentVoteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VoteVideoCommentCommandHandler (
            IVideoCommentRepository commentRepository,
            IVideoCommentVoteRepository commentVoteRepository,
            IUnitOfWork unitOfWork) {
            _commentRepository = commentRepository;
            _commentVoteRepository = commentVoteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (VoteVideoCommentCommand request, CancellationToken cancellationToken) {
            var comment = await _commentRepository.GetVideoCommentByIdAsync(request.CommentId, false);

            if (comment == null) {
                throw new AppException("Comment does not exist.", null, StatusCodes.Status404NotFound);
            }

            try {
                await _unitOfWork.ExecuteOptimisticTransactionAsync(async () => {
                    var commentVote = await _commentVoteRepository.GetVideoCommentVoteAsync(request.UserId, request.CommentId);

                    if (commentVote == null) {
                        commentVote = VideoCommentVote.Create(request.UserId, request.CommentId, comment.VideoId, request.VoteType);

                        await _commentVoteRepository.AddVideoCommentVoteAsync(commentVote);
                        await _unitOfWork.CommitAsync();
                    } else {
                        if (request.VoteType != commentVote.Type) {
                            commentVote.ChangeVoteType(request.VoteType);
                            commentVote.IncrementVersion();
                            await _unitOfWork.CommitAsync();
                        }
                    }
                });
            } catch (Exception ex) {
                if (ex.Identify(ExceptionCategories.UniqueViolation)) {
                    throw new AppException("Vote is already created.", null, StatusCodes.Status400BadRequest);
                }

                throw;
            }

            return Unit.Value;
        }

    }
}
