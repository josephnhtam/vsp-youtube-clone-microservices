using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using Library.Domain.DomainEvents.Videos;
using Library.Domain.Models;
using MongoDB.Driver;

namespace Library.Infrastructure.DomainEventHandlers.Videos {
    public class VideoVoteMetricsChangedDomainEventHandler : IDomainEventHandler<VideoVoteMetricsChangedDomainEvent> {

        private readonly IMongoCollectionContext<Video> _context;

        public VideoVoteMetricsChangedDomainEventHandler (IMongoCollectionContext<Video> context) {
            _context = context;
        }

        public Task Handle (VideoVoteMetricsChangedDomainEvent @event, CancellationToken cancellationToken) {
            var filterBuilder = Builders<Video>.Filter;

            var filter =
                filterBuilder.Eq(nameof(Video.Id), @event.VideoId);

            var update = Builders<Video>.Update
                .Inc($"{nameof(Video.Metrics)}.{nameof(VideoMetrics.LikesCount)}", @event.LikesCountChange)
                .Inc($"{nameof(Video.Metrics)}.{nameof(VideoMetrics.DislikesCount)}", @event.DislikesCountChange);

            if (@event.NextSyncDate.HasValue) {
                update = update.Set($"{nameof(Video.Metrics)}.{nameof(VideoMetrics.NextSyncDate)}", @event.NextSyncDate.Value);
            }

            _context.UpdateOne(filter, update);
            return Task.CompletedTask;
        }
    }
}
