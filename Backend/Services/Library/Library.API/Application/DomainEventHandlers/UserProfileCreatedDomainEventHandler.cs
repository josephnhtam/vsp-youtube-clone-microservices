using Domain.Events;
using Library.Domain.Contracts;
using Library.Domain.DomainEvents.UserProfiles;
using Library.Domain.Models;

namespace Library.API.Application.DomainEventHandlers {
    public class UserProfileCreatedDomainEventHandler : IDomainEventHandler<UserProfileCreatedDomainEvent> {

        private readonly IUniquePlaylistRepository<LikedPlaylist, PlaylistItem> _likedPlaylistRepository;
        private readonly IUniquePlaylistRepository<DislikedPlaylist, PlaylistItem> _dislikedPlaylistRepository;
        private readonly IUniquePlaylistRepository<WatchLaterPlaylist, OrderedPlaylistItem> _watchLaterPlaylistRepository;

        public UserProfileCreatedDomainEventHandler (
            IUniquePlaylistRepository<LikedPlaylist, PlaylistItem> likedPlaylistRepository,
            IUniquePlaylistRepository<DislikedPlaylist, PlaylistItem> dislikedPlaylistRepository,
            IUniquePlaylistRepository<WatchLaterPlaylist, OrderedPlaylistItem> watchLaterPlaylistRepository) {
            _likedPlaylistRepository = likedPlaylistRepository;
            _dislikedPlaylistRepository = dislikedPlaylistRepository;
            _watchLaterPlaylistRepository = watchLaterPlaylistRepository;
        }

        public async Task Handle (UserProfileCreatedDomainEvent @event, CancellationToken cancellationToken) {
            var likedPlaylist = LikedPlaylist.Create(@event.UserProfile.Id);
            var dislikedPlaylist = DislikedPlaylist.Create(@event.UserProfile.Id);
            var watchLaterPlaylist = WatchLaterPlaylist.Create(@event.UserProfile.Id);

            await _likedPlaylistRepository.AddPlaylistAsync(likedPlaylist);
            await _dislikedPlaylistRepository.AddPlaylistAsync(dislikedPlaylist);
            await _watchLaterPlaylistRepository.AddPlaylistAsync(watchLaterPlaylist);
        }

    }
}
