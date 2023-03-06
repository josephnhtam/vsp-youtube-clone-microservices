using Community.Domain.Contracts;
using Community.Domain.DomainEvents;
using Domain.Events;

namespace Community.API.Application.DomainEventHandlers {
    public class VideoForumUnregisteredDomainEventHandler : IDomainEventHandler<VideoForumUnregisteredDomainEvent> {

        private readonly IVideoForumRepository _videoForumRepository;

        public VideoForumUnregisteredDomainEventHandler (IVideoForumRepository videoForumRepository) {
            _videoForumRepository = videoForumRepository;
        }

        public async Task Handle (VideoForumUnregisteredDomainEvent @event, CancellationToken cancellationToken) {
            await _videoForumRepository.RemoveVideoForumAsync(@event.VideoForum);
        }

    }
}
