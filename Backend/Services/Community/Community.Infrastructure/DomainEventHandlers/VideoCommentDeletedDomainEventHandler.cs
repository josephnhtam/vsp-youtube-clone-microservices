using Community.Domain.Contracts;
using Community.Domain.DomainEvents;
using Domain.Contracts;
using Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Community.Infrastructure.DomainEventHandlers {
    public class VideoCommentDeletedDomainEventHandler : IDomainEventHandler<VideoCommentDeletedDomainEvent> {

        private readonly CommunityDbContext _dbContext;
        private readonly IVideoCommentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public VideoCommentDeletedDomainEventHandler (CommunityDbContext dbContext, IVideoCommentRepository repository, IUnitOfWork unitOfWork) {
            _dbContext = dbContext;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle (VideoCommentDeletedDomainEvent @event, CancellationToken cancellationToken) {
            if (_dbContext.Database.CurrentTransaction == null) {
                throw new InvalidOperationException("Transaction is required for this operation");
            }

            var videoComment = @event.VideoComment;

            await _repository.DeleteVideoCommentAsync(videoComment);
            await _unitOfWork.CommitAsync(cancellationToken);

            await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE ""VideoForums"" SET ""VideoCommentsCount"" = (SELECT COUNT(*) FROM ""VideoComments"" WHERE ""VideoId"" = {videoComment.VideoId}) WHERE ""VideoId"" = {videoComment.VideoId}");

            if (videoComment.ParentCommentId != null) {
                await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $@"UPDATE ""VideoComments"" SET ""RepliesCount"" = ""RepliesCount"" - 1 WHERE ""Id"" = {videoComment.ParentCommentId}");
            } else {
                await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $@"UPDATE ""VideoForums"" SET ""RootVideoCommentsCount"" = ""RootVideoCommentsCount"" - 1 WHERE ""VideoId"" = {videoComment.VideoId}");
            }
        }

    }
}
