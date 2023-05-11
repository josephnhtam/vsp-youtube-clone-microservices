using AutoMapper;
using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using EventBus.Helper.RoutingSlips.Extensions;
using VideoManager.API.Application.DtoModels;
using VideoManager.API.Application.IntegrationEvents;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.DomainEvents;

namespace VideoManager.API.Application.DomainEventHandlers {
    public class VideoUploadedDomainEventHandler : IDomainEventHandler<VideoUploadedDomainEvent> {

        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly IMapper _mapper;

        public VideoUploadedDomainEventHandler (
            IUserProfileRepository userProfileRepository,
            ITransactionalEventsContext transactionalEventsContext,
            IMapper mapper) {
            _userProfileRepository = userProfileRepository;
            _transactionalEventsContext = transactionalEventsContext;
            _mapper = mapper;
        }

        public async Task Handle (VideoUploadedDomainEvent @event, CancellationToken cancellationToken) {
            var video = @event.Video;

            var creatorProfile = await _userProfileRepository.GetUserProfileAsync(video.CreatorId);
            if (creatorProfile == null) throw new Exception("Creator profile not found");

            _transactionalEventsContext.AddOutboxMessage(
                new NotifyVideoUploadedIntegrationEvent(
                    video.Id,
                    video.CreatorId,
                    video.OriginalVideoFileName!,
                    video.OriginalVideoUrl!));

            // Register video to other services using distributed transaction
            _transactionalEventsContext.AddRoutingSlip(builder => {
                var creatorProfileDto = _mapper.Map<InternalUserProfileDto>(creatorProfile);

                var videoRegistrationProperties = new VideoRegistartionPropertiesDto {
                    VideoId = video.Id,
                    Title = video.Title,
                    Description = video.Description,
                    Tags = video.Tags,
                    Visibility = video.Visibility,
                    CreateDate = video.CreateDate,
                    CreatorProfile = creatorProfileDto
                };

                var videoForumCreationProperties = new VideoForumCreationPropertiesDto {
                    VideoId = video.Id,
                    CreatorProfile = creatorProfileDto,
                    AllowedToComment = true
                };

                builder.AddCheckpoint("RegisterVideoToLibrary",
                    "Library.VideoRegistration", videoRegistrationProperties);

                builder.AddCheckpoint("RegisterVideoToHistory",
                    "History.VideoRegistration", videoRegistrationProperties);

                builder.AddCheckpoint("RegisterVideoToCommunity",
                    "Community.VideoRegistration", videoForumCreationProperties);

                builder.AddCheckpoint("RegisterVideoToStore",
                   "VideoStore.VideoRegistration", videoRegistrationProperties);

                builder.AddCheckpoint("CompleteVideoRegistration",
                    "VideoManager.VideoRegistered", videoRegistrationProperties);

                builder.SetRollbackDestination("FailVideoRegistration",
                    "VideoManager.RegistrationFailed", videoRegistrationProperties);
            });
        }

    }
}
