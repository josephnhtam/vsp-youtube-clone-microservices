using AutoMapper;
using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using EventBus.Helper.RoutingSlips.Extensions;
using Users.API.Application.DtoModels;
using Users.Domain.Contracts;
using Users.Domain.DomainEvents;
using Users.Domain.Models;

namespace Users.API.Application.DomainEventHandlers {
    public class UserProfileCreatedDomainEventHandler : IDomainEventHandler<UserProfileCreatedDomainEvent> {

        private readonly IUserChannelRepository _channelRepository;
        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly IMapper _mapper;

        public UserProfileCreatedDomainEventHandler (
            IUserChannelRepository channelRepository,
            ITransactionalEventsContext transactionalEventsContext,
            IMapper mapper) {
            _channelRepository = channelRepository;
            _transactionalEventsContext = transactionalEventsContext;
            _mapper = mapper;
        }

        public async Task Handle (UserProfileCreatedDomainEvent @event, CancellationToken cancellationToken) {
            var userChannel = UserChannel.Create(@event.UserProfile.Id, @event.UserProfile.Handle);
            await _channelRepository.AddUserChannelAsync(userChannel);

            var userProfile = _mapper.Map<InternalUserProfileDto>(@event.UserProfile);
            _transactionalEventsContext.AddRoutingSlip(builder => {
                builder.AddCheckpoint("RegisterToLibrary",
                    "Library.UserProfileRegistration", userProfile);

                builder.AddCheckpoint("RegisterToSubscriptions",
                    "Subscriptions.UserProfileRegistration", userProfile);

                builder.AddCheckpoint("RegisterToCommunity",
                    "Community.UserProfileRegistration", userProfile);

                builder.AddCheckpoint("RegisterToHistory",
                    "History.UserProfileRegistration", userProfile);

                builder.AddCheckpoint("RegisterToVideoManager",
                    "VideoManager.UserProfileRegistration", userProfile);

                builder.AddCheckpoint("RegisterToVideoStore",
                    "VideoStore.UserProfileRegistration", userProfile);

                builder.AddCheckpoint("RegisterToVideoStore",
                    "VideoStore.UserProfileRegistration", userProfile);

                builder.AddCheckpoint("CompleteRegistration",
                    "Users.UserProfileRegistered", userProfile);

                builder.SetRollbackDestination("FailRegistration",
                    "Users.UserProfileRegistrationFailed", userProfile);
            });
        }
    }
}
