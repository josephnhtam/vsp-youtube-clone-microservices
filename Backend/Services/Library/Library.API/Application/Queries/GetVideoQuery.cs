using Application.Contracts;
using Library.API.Application.DtoModels;

namespace Library.API.Application.Queries {
    public class GetVideoQuery : IQuery<VideoDto?> {
        public Guid VideoId { get; set; }
        public bool PublicOnly { get; set; }

        public GetVideoQuery (Guid videoId, bool publicOnly) {
            VideoId = videoId;
            PublicOnly = publicOnly;
        }
    }
}
