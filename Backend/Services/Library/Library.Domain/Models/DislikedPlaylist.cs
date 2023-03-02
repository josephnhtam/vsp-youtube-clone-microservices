namespace Library.Domain.Models {
    public class DislikedPlaylist : UnorderedPlaylistBase<DislikedPlaylist> {

        protected DislikedPlaylist () { }

        protected DislikedPlaylist (Guid id, string userId) : base(id, userId) {
        }

        public static DislikedPlaylist Create (string userId) {
            Guid id = Guid.NewGuid();
            return new DislikedPlaylist(id, userId);
        }

    }
}
