using Community.Domain.DomainEvents;
using Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Community.Infrastructure.DomainEventHandlers {
    public class VideoCommentCreatedDomainEventHandler : IDomainEventHandler<VideoCommentCreatedDomainEvent> {

        private readonly CommunityDbContext _dbContext;

        public VideoCommentCreatedDomainEventHandler (CommunityDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task Handle (VideoCommentCreatedDomainEvent @event, CancellationToken cancellationToken) {
            if (_dbContext.Database.CurrentTransaction == null) {
                throw new InvalidOperationException("Transaction is required for this operation");
            }

            var videoComment = @event.VideoComment;

            if (videoComment.ParentCommentId != null) {
                await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $@"UPDATE ""VideoForums"" SET ""VideoCommentsCount"" = ""VideoCommentsCount"" + 1 WHERE ""VideoId"" = {videoComment.VideoId}");

                await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $@"UPDATE ""VideoComments"" SET ""RepliesCount"" = ""RepliesCount"" + 1 WHERE ""Id"" = {videoComment.ParentCommentId}");
            } else {
                await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $@"UPDATE ""VideoForums"" SET ""RootVideoCommentsCount"" = ""RootVideoCommentsCount"" + 1, ""VideoCommentsCount"" = ""VideoCommentsCount"" + 1 WHERE ""VideoId"" = {videoComment.VideoId}");
            }
        }

    }
}
