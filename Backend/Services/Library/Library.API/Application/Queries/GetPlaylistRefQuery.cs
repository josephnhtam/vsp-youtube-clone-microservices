using Application.Contracts;
using Library.Domain.Models;

namespace Library.API.Application.Queries {
    public class GetPlaylistRefQuery : IQuery<PlaylistRef?> {
        public string UserId { get; set; }
        public Guid PlaylistId { get; set; }

        public GetPlaylistRefQuery (string userId, Guid playlistId) {
            UserId = userId;
            PlaylistId = playlistId;
        }
    }
}
