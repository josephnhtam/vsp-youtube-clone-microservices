namespace Library.Domain.Models {
    public class LikedPlaylist : UnorderedPlaylistBase<LikedPlaylist> {

        protected LikedPlaylist () { }

        protected LikedPlaylist (Guid id, string userId) : base(id, userId) {
        }

        public static LikedPlaylist Create (string userId) {
            Guid id = Guid.NewGuid();
            return new LikedPlaylist(id, userId);
        }

    }
}
