using Application.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Queries {
    public class GetVideoQuery : IQuery<Video> {
        public Guid VideoId { get; set; }

        public GetVideoQuery (Guid videoId) {
            VideoId = videoId;
        }
    }
}
