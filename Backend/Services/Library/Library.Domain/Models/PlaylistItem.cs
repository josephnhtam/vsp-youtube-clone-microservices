using Domain;

namespace Library.Domain.Models {
    public class PlaylistItem : ValueObject {

        public Guid Id { get; set; }
        public Guid VideoId { get; private set; }
        public DateTimeOffset CreateDate { get; private set; }

        public PlaylistItem (Guid id, Guid videoId, DateTimeOffset createDate) {
            Id = id;
            VideoId = videoId;
            CreateDate = createDate;
        }

    }
}
