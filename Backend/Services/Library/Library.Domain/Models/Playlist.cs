using Library.Domain.DomainEvents.Playlists;
using Library.Domain.Rules.Playlists;

namespace Library.Domain.Models {
    public class Playlist : OrderedPlaylistBase<Playlist> {

        public string Title { get; private set; }
        public string Description { get; private set; }
        public PlaylistVisibility Visibility { get; private set; }

        protected Playlist () { }

        protected Playlist (Guid id, string userId, string title, string description, PlaylistVisibility visibility) : base(id, userId) {
            CheckRules(new TitleLengthRule(title),
                       new DescriptionLengthRule(description),
                       new ValidPlaylistVisibilityRule(visibility));

            Title = title;
            Description = description;
            Visibility = visibility;
        }

        public static Playlist Create (string userId, string title, string description, PlaylistVisibility visibility) {
            Guid id = Guid.NewGuid();
            var playlist = new Playlist(id, userId, title, description, visibility);
            return playlist;
        }

        public void Update (string? title, string? description, PlaylistVisibility? visibility) {
            bool updated = false;

            if (title != null) {
                CheckRule(new TitleLengthRule(Title));
                Title = title;
                updated = true;
            }

            if (description != null) {
                CheckRule(new DescriptionLengthRule(Description));
                Description = description;
                updated = true;
            }

            if (visibility.HasValue) {
                CheckRule(new ValidPlaylistVisibilityRule(visibility.Value));
                Visibility = visibility.Value;
                updated = true;
            }

            if (updated) {
                UpdateDate = DateTimeOffset.UtcNow;
                AddDomainEvent(new UpdatePlaylistDomainEvent(Id, title, description, visibility, UpdateDate));
            }
        }

    }
}
