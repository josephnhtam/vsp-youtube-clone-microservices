using Community.Domain.Models;

namespace Community.API.Application.DtoModels {
    public class VoteVideoCommentRequestDto {
        public long CommentId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
