using Application.Contracts;
using Library.API.Application.DtoModels;

namespace Library.API.Application.Queries {
    public class GetPublicSimplePlaylistInfosByIdsQuery : IQuery<List<SimplePlaylistInfoDto>> {
        public List<Guid> PlaylistIds { get; set; }

        public GetPublicSimplePlaylistInfosByIdsQuery (List<Guid> playlistIds) {
            PlaylistIds = playlistIds;
        }
    }
}
