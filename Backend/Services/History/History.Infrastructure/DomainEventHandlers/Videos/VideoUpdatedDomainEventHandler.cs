using Domain.Events;
using History.Domain.DomainEvents.Videos;
using History.Domain.Models;
using Infrastructure.MongoDb.Contexts;
using MongoDB.Driver;

namespace History.Infrastructure.DomainEventHandlers.Videos {
    public class VideoUpdatedDomainEventHandler : IDomainEventHandler<VideoUpdatedDomainEvent> {

        private readonly IMongoCollectionContext<Video> _context;

        public VideoUpdatedDomainEventHandler (IMongoCollectionContext<Video> context) {
            _context = context;
        }

        public Task Handle (VideoUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            var filterBuilder = Builders<Video>.Filter;

            var filter = filterBuilder.Eq(nameof(Video.Id), @event.VideoId) &
                         filterBuilder.Lt(nameof(Video.VideoVersion), @event.Version);

            var update = Builders<Video>.Update
                .Set(nameof(Video.Title), @event.Title)
                .Set(nameof(Video.Description), @event.Description)
                .Set(nameof(Video.Tags), @event.Tags)
                .Set(nameof(Video.ThumbnailUrl), @event.ThumbnailUrl)
                .Set(nameof(Video.PreviewThumbnailUrl), @event.PreviewThumbnailUrl)
                .Set(nameof(Video.LengthSeconds), @event.LengthSeconds)
                .Set(nameof(Video.Visibility), @event.Visibility)
                .Set(nameof(Video.Status), @event.Status)
                .Set(nameof(Video.VideoVersion), @event.Version);

            _context.UpdateOne(filter, update);
            return Task.CompletedTask;
        }
    }
}
