namespace Community.Domain.Models {
    public class UserVideoCommentVote {
        public long CommentId { get; set; }
        public VoteType VoteType { get; set; }

        public UserVideoCommentVote (long commentId, VoteType voteType) {
            CommentId = commentId;
            VoteType = voteType;
        }
    }
}
