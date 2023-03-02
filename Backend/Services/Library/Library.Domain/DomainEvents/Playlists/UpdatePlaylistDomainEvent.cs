using Domain.Events;
using Library.Domain.Models;

namespace Library.Domain.DomainEvents.Playlists {
    public class UpdatePlaylistDomainEvent : IDomainEvent {
        public Guid PlaylistId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public PlaylistVisibility? Visibility { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public UpdatePlaylistDomainEvent (Guid playlistId, string? title, string? description, PlaylistVisibility? visibility, DateTimeOffset updateDate) {
            PlaylistId = playlistId;
            Title = title;
            Description = description;
            Visibility = visibility;
            UpdateDate = updateDate;
        }
    }
}
