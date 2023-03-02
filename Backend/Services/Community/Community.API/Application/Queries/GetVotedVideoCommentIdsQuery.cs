using Application.Contracts;
using Community.Domain.Models;

namespace Community.API.Application.Queries {
    public class GetVotedVideoCommentIdsQuery : IQuery<List<UserVideoCommentVote>> {
        public Guid VideoId { get; set; }
        public string UserId { get; set; }

        public GetVotedVideoCommentIdsQuery (Guid videoId, string userId) {
            VideoId = videoId;
            UserId = userId;
        }
    }
}
