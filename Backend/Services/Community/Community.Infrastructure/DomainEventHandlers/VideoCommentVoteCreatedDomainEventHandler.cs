using Community.Domain.DomainEvents;
using Community.Domain.Models;
using Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Community.Infrastructure.DomainEventHandlers {
    public class VideoCommentVoteCreatedDomainEventHandler : IDomainEventHandler<VideoCommentVoteCreatedDomainEvent> {

        private readonly CommunityDbContext _dbContext;

        public VideoCommentVoteCreatedDomainEventHandler (CommunityDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task Handle (VideoCommentVoteCreatedDomainEvent @event, CancellationToken cancellationToken) {
            if (_dbContext.Database.CurrentTransaction == null) {
                throw new InvalidOperationException("Transaction is required for this operation");
            }

            var videoCommentVote = @event.VideoCommentVote;

            switch (videoCommentVote.Type) {
                case VoteType.Like:
                    await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                        $@"UPDATE ""VideoComments"" SET ""LikesCount"" = GREATEST(0, ""LikesCount"" + 1) WHERE ""Id"" = {videoCommentVote.VideoCommentId}");
                    break;
                case VoteType.Dislike:
                    await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                        $@"UPDATE ""VideoComments"" SET ""DislikesCount"" = GREATEST(0, ""DislikesCount"" + 1) WHERE ""Id"" = {videoCommentVote.VideoCommentId}");
                    break;
            }
        }

    }
}
