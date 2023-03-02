using Application.Contracts;
using Library.API.Application.DtoModels;

namespace Library.API.Application.Queries {
    public class GetPlaylistsWithVideoQuery : IQuery<PlaylistsWithVideoDto> {
        public string UserId { get; set; }
        public Guid VideoId { get; set; }

        public GetPlaylistsWithVideoQuery (string userId, Guid videoId) {
            UserId = userId;
            VideoId = videoId;
        }
    }
}
