using Application.Contracts;

namespace Community.API.Application.Commands {
    public class EditVideoCommentCommand : ICommand {
        public string UserId { get; set; }
        public long CommentId { get; set; }
        public string Comment { get; set; }

        public EditVideoCommentCommand (string userId, long commentId, string comment) {
            UserId = userId;
            CommentId = commentId;
            Comment = comment;
        }
    }
}
