namespace Library.Domain.Models {
    public class OrderedPlaylistItem : PlaylistItem {

        public int Position { get; private set; }

        public OrderedPlaylistItem (Guid id, Guid videoId, int position, DateTimeOffset createDate) : base(id, videoId, createDate) {
            Position = position;
        }

        public void SetPosition (int newPosition) {
            Position = newPosition;
        }

    }
}
