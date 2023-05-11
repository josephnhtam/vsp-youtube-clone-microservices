using Application.Handlers;
using Community.Domain.Contracts;
using Domain.Contracts;
using MediatR;
using SharedKernel.Exceptions;

namespace Community.API.Application.Commands.Handlers {
    public class EditVideoCommentCommandHandler : ICommandHandler<EditVideoCommentCommand> {

        private readonly IVideoCommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditVideoCommentCommandHandler (
            IVideoCommentRepository commentRepository,
            IUnitOfWork unitOfWork) {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (EditVideoCommentCommand request, CancellationToken cancellationToken) {
            var comment = await _commentRepository.GetVideoCommentByIdAsync(request.CommentId, false);

            if (comment == null) {
                throw new AppException("Comment does not exist.", null, StatusCodes.Status404NotFound);
            }

            if (comment.UserId != request.UserId) {
                throw new AppException("Unauthorized", null, StatusCodes.Status401Unauthorized);
            }

            comment.Edit(request.Comment);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
