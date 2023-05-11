using Application.Handlers;
using Community.Domain.Contracts;
using Domain.Contracts;
using MediatR;
using SharedKernel.Exceptions;

namespace Community.API.Application.Commands.Handlers {
    public class DeleteVideoCommentCommandHandler : ICommandHandler<DeleteVideoCommentCommand> {

        private readonly IVideoCommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteVideoCommentCommandHandler (
            IVideoCommentRepository commentRepository,
            IUnitOfWork unitOfWork) {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (DeleteVideoCommentCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteTransactionAsync(async () => {
                var comment = await _commentRepository.GetVideoCommentByIdAsync(request.CommentId, false);

                if (comment == null) {
                    throw new AppException("Comment does not exist.", null, StatusCodes.Status404NotFound);
                }

                if (comment.UserId != request.UserId) {
                    throw new AppException("Unauthorized", null, StatusCodes.Status401Unauthorized);
                }

                comment.Delete();

                await _unitOfWork.CommitAsync(cancellationToken);
            }, null, cancellationToken);

            return Unit.Value;
        }

    }
}
