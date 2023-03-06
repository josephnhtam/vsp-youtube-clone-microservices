using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using Library.Domain.DomainEvents.Videos;
using Library.Domain.Models;
using MongoDB.Driver;

namespace Library.Infrastructure.DomainEventHandlers.Videos {
    public class VideoViewsMetricsUpdatedDomainEventHandler : IDomainEventHandler<VideoViewsMetricsUpdatedDomainEvent> {

        private readonly IMongoCollectionContext<Video> _context;

        public VideoViewsMetricsUpdatedDomainEventHandler (IMongoCollectionContext<Video> context) {
            _context = context;
        }

        public Task Handle (VideoViewsMetricsUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            var filterBuilder = Builders<Video>.Filter;

            var filter =
                filterBuilder.Eq(nameof(Video.Id), @event.VideoId) &
                (filterBuilder.Eq<DateTimeOffset?>($"{nameof(Video.Metrics)}.{nameof(VideoMetrics.ViewsCountUpdateDate)}", null) |
                 filterBuilder.Lt($"{nameof(Video.Metrics)}.{nameof(VideoMetrics.ViewsCountUpdateDate)}", @event.UpdateDate));

            var update = Builders<Video>.Update
                .Set($"{nameof(Video.Metrics)}.{nameof(VideoMetrics.ViewsCount)}", @event.ViewsCount)
                .Set($"{nameof(Video.Metrics)}.{nameof(VideoMetrics.ViewsCountUpdateDate)}", @event.UpdateDate);

            _context.UpdateOne(filter, update);
            return Task.CompletedTask;
        }
    }
}
