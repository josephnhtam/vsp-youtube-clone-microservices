using Domain.Events;
using History.Domain.DomainEvents.Videos;
using History.Domain.Models;
using Infrastructure.MongoDb.Contexts;
using MongoDB.Driver;

namespace History.Infrastructure.DomainEventHandlers.Videos {
    public class VideoViewsMetricsSyncDateUpdatedDomainEventHandler : IDomainEventHandler<VideoViewsMetricsSyncDateUpdatedDomainEvent> {

        private readonly IMongoCollectionContext<Video> _context;

        public VideoViewsMetricsSyncDateUpdatedDomainEventHandler (IMongoCollectionContext<Video> context) {
            _context = context;
        }

        public Task Handle (VideoViewsMetricsSyncDateUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            var filterBuilder = Builders<Video>.Filter;

            var filter = filterBuilder.Eq(nameof(Video.Id), @event.VideoId);

            var update = Builders<Video>.Update
                .Set($"{nameof(Video.Metrics)}.{nameof(VideoMetrics.NextSyncDate)}", @event.NextSyncDate);

            _context.UpdateOne(filter, update);
            return Task.CompletedTask;
        }
    }
}
