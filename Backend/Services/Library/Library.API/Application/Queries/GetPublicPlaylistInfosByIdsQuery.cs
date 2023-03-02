using Application.Contracts;
using Library.API.Application.DtoModels;

namespace Library.API.Application.Queries {
    public class GetPublicPlaylistInfosByIdsQuery : IQuery<List<PlaylistInfoDto>> {
        public string? UserId { get; set; }
        public List<Guid> PlaylistIds { get; set; }

        public GetPublicPlaylistInfosByIdsQuery (string? userId, List<Guid> playlistIds) {
            UserId = userId;
            PlaylistIds = playlistIds;
        }
    }
}
