using Domain.Events;
using History.Domain.DomainEvents.Videos;
using History.Domain.Models;
using Infrastructure.MongoDb.Contexts;
using MongoDB.Driver;

namespace History.Infrastructure.DomainEventHandlers.Videos {
    public class VideoViewsMetricsChangedDomainEventHandler : IDomainEventHandler<VideoViewsMetricsChangedDomainEvent> {

        private readonly IMongoCollectionContext<Video> _context;

        public VideoViewsMetricsChangedDomainEventHandler (IMongoCollectionContext<Video> context) {
            _context = context;
        }

        public Task Handle (VideoViewsMetricsChangedDomainEvent @event, CancellationToken cancellationToken) {
            var filterBuilder = Builders<Video>.Filter;

            var filter = filterBuilder.Eq(nameof(Video.Id), @event.VideoId);

            var update = Builders<Video>.Update
                .Inc($"{nameof(Video.Metrics)}.{nameof(VideoMetrics.ViewsCount)}", @event.ViewsCountChange);

            _context.UpdateOne(filter, update);
            return Task.CompletedTask;
        }
    }
}
