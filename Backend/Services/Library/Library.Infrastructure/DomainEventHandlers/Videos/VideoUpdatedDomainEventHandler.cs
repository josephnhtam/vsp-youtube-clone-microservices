using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using Library.Domain.DomainEvents.Videos;
using Library.Domain.Models;
using MongoDB.Driver;

namespace Library.Infrastructure.DomainEventHandlers.Videos {
    public class VideoUpdatedDomainEventHandler : IDomainEventHandler<VideoUpdatedDomainEvent> {

        private readonly IMongoCollectionContext<Video> _videoContext;
        private readonly IMongoCollectionContext<UserProfile> _userProfileContext;

        public VideoUpdatedDomainEventHandler (IMongoCollectionContext<Video> videoContext, IMongoCollectionContext<UserProfile> userProfileContext) {
            _videoContext = videoContext;
            _userProfileContext = userProfileContext;
        }

        public Task Handle (VideoUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            {
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
                    .Set(nameof(Video.StatusUpdateDate), @event.PublishDate)
                    .Set(nameof(Video.VideoVersion), @event.Version);

                _videoContext.UpdateOne(filter, update);
            }

            {
                var filter = Builders<UserProfile>.Filter.Eq(nameof(UserProfile.Id), @event.CreatorId);

                var update = Builders<UserProfile>.Update
                    .Set(nameof(UserProfile.LastVideoUpdateDate), DateTimeOffset.UtcNow);

                _userProfileContext.UpdateOne(filter, update);
            }

            return Task.CompletedTask;
        }
    }
}
