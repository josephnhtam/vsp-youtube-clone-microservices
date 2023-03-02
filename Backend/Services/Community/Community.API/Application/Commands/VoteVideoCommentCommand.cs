using Application.Contracts;
using Community.Domain.Models;

namespace Community.API.Application.Commands {
    public class VoteVideoCommentCommand : ICommand {
        public long CommentId { get; set; }
        public string UserId { get; set; }
        public VoteType VoteType { get; set; }

        public VoteVideoCommentCommand (long commentId, string userId, VoteType attitudeType) {
            CommentId = commentId;
            UserId = userId;
            VoteType = attitudeType;
        }
    }
}
