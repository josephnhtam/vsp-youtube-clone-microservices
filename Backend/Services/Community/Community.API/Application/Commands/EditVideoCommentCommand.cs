using Application.Contracts;

namespace Community.API.Application.Commands {
    public class DeleteVideoCommentCommand : ICommand {
        public string UserId { get; set; }
        public long CommentId { get; set; }

        public DeleteVideoCommentCommand (string userId, long commentId) {
            UserId = userId;
            CommentId = commentId;
        }
    }
}
