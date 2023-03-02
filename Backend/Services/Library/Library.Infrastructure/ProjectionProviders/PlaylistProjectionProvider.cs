using Library.Domain.Models;

namespace Library.Infrastructure.ProjectionProviders {
    public class PlaylistProjectionProvider : IPlaylistProjectionProvider<Playlist> {
        public IEnumerable<string> GetAdditionalProjectionFields () {
            return new[] {
                nameof(Playlist.Title),
                nameof(Playlist.Description),
                nameof(Playlist.Visibility)
            };
        }
    }
}
