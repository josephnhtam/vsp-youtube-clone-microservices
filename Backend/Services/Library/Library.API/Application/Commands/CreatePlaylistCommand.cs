using Application.Contracts;
using Library.Domain.Models;

namespace Library.API.Application.Commands {
    public class CreatePlaylistCommand : ICommand<Guid> {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public PlaylistVisibility Visibility { get; set; }

        public CreatePlaylistCommand (string userId, string title, string description, PlaylistVisibility visibility) {
            UserId = userId;
            Title = title;
            Description = description;
            Visibility = visibility;
        }
    }
}
