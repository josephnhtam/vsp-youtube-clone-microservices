using Application.Contracts;
using VideoStore.Domain.Models;

namespace VideoStore.API.Application.Queries {
    public class GetVideoQuery : IQuery<Video> {
        public string? UserId { get; set; }
        public Guid VideoId { get; set; }

        public GetVideoQuery (string? userId, Guid videoId) {
            UserId = userId;
            VideoId = videoId;
        }
    }
}
