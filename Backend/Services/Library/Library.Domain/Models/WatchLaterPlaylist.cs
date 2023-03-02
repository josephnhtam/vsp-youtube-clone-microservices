namespace Library.Domain.Models {
    public class WatchLaterPlaylist : OrderedPlaylistBase<WatchLaterPlaylist> {

        protected WatchLaterPlaylist () { }

        protected WatchLaterPlaylist (Guid id, string userId) : base(id, userId) {
        }

        public static WatchLaterPlaylist Create (string userId) {
            Guid id = Guid.NewGuid();
            return new WatchLaterPlaylist(id, userId);
        }

    }
}
