using Library.Domain.Models;

namespace Library.Infrastructure.ProjectionProviders {
    public interface IPlaylistProjectionProvider<TPlaylist> where TPlaylist : PlaylistBase {
        IEnumerable<string> GetAdditionalProjectionFields ();
    }
}
