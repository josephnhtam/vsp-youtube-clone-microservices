using Application.Contracts;
using Community.Domain.Models;

namespace Community.API.Application.Queries {
    public class GetUserRootVideoCommentsQuery : IQuery<List<VideoComment>> {
        public Guid VideoId { get; set; }
        public string UserId { get; set; }
        public int MaxCount { get; set; }

        public GetUserRootVideoCommentsQuery (Guid videoId, string userId, int maxCount) {
            VideoId = videoId;
            UserId = userId;
            MaxCount = maxCount;
        }
    }
}
