using Domain.Rules;
using Library.Domain.Models;

namespace Library.Domain.Rules.Playlists {
    public class ValidPlaylistVisibilityRule : DefinedEnumRule<PlaylistVisibility> {
        public ValidPlaylistVisibilityRule (PlaylistVisibility visibility) : base(visibility, "Visibility") { }
    }
}
