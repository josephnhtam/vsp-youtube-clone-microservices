using Domain;

namespace Library.Domain.Models {
    public class PlaylistRef : DomainEntity, IAggregateRoot {

        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public Guid PlaylistId { get; private set; }
        public DateTimeOffset CreateDate { get; private set; }

        private PlaylistRef (Guid id, string userId, Guid playlistId, DateTimeOffset date) {
            Id = id;
            UserId = userId;
            PlaylistId = playlistId;
            CreateDate = date;
        }

        public static PlaylistRef Create (string userId, Guid playlistId, DateTimeOffset date) {
            Guid id = Guid.NewGuid();
            return new PlaylistRef(id, userId, playlistId, date);
        }

    }
}
