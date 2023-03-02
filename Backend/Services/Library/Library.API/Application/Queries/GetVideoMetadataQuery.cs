using Application.Contracts;
using Library.API.Application.DtoModels;

namespace Library.API.Application.Queries {
    public class GetVideoMetadataQuery : IQuery<VideoMetadataDto?> {
        public Guid VideoId { get; set; }
        public string? UserId { get; set; }

        public GetVideoMetadataQuery (Guid videoId, string? userId) {
            VideoId = videoId;
            UserId = userId;
        }
    }
}
