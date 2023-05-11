using AutoMapper;
using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using Library.API.Application.DtoModels;
using Library.API.Application.IntegrationEvents;
using Library.Domain.Contracts;
using Library.Domain.DomainEvents.Videos;
using Library.Domain.Models;

namespace Library.API.Application.DomainEventHandlers {
    public class VideoSearchSearchInfoUpdatedDomainEventHandler : IDomainEventHandler<VideoSearchInfoUpdatedDomainEvent> {

        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly IMapper _mapper;

        public VideoSearchSearchInfoUpdatedDomainEventHandler (
            IUserProfileRepository userProfileRepository,
            ITransactionalEventsContext transactionalEventsContext,
            IMapper mapper) {
            _userProfileRepository = userProfileRepository;
            _transactionalEventsContext = transactionalEventsContext;
            _mapper = mapper;
        }

        public async Task Handle (VideoSearchInfoUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            var video = @event.Video;

            if (video.Status == VideoStatus.Unregistered) {
                _transactionalEventsContext.AddOutboxMessage(
                        new RemoveVideoSearchInfoIntegrationEvent(
                            video.Id,
                            video.VideoVersion + 1));
            } else {
                if (video.IsPublic) {
                    var userProfile = await _userProfileRepository.GetUserProfileAsync(video.CreatorId);

                    if (userProfile != null) {
                        _transactionalEventsContext.AddOutboxMessage(
                            new CreateOrUpdateVideoSearchInfoIntegrationEvent(
                                video.Id,
                                _mapper.Map<InternalUserProfileDto>(userProfile),
                                video.Title,
                                video.Description,
                                video.ThumbnailUrl,
                                video.PreviewThumbnailUrl,
                                video.Tags.Split(",").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList(),
                                video.LengthSeconds,
                                _mapper.Map<VideoMetricsDto>(video.Metrics),
                                video.CreateDate,
                                video.VideoVersion));
                    }
                } else {
                    _transactionalEventsContext.AddOutboxMessage(
                        new RemoveVideoSearchInfoIntegrationEvent(
                            video.Id,
                            video.VideoVersion));
                }
            }
        }

    }
}
