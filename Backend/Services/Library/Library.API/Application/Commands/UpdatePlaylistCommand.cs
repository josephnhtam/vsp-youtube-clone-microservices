using Application.Contracts;
using Library.Domain.Models;

namespace Library.API.Application.Commands {
    public class UpdatePlaylistCommand : ICommand {
        public string UserId { get; set; }
        public Guid PlaylistId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public PlaylistVisibility? Visibility { get; set; }

        public UpdatePlaylistCommand (string userId, Guid playlistId, string? title, string? description, PlaylistVisibility? visibility) {
            UserId = userId;
            PlaylistId = playlistId;
            Title = title;
            Description = description;
            Visibility = visibility;
        }
    }
}
