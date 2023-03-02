using Application.Contracts;
using Community.Domain.Models;

namespace Community.API.Application.Commands {
    public class AddVideoCommentCommand : ICommand<VideoComment> {
        public string UserId { get; set; }
        public string Comment { get; set; }
        public Guid? VideoId { get; set; }
        public long? ParentCommentId { get; set; }

        public AddVideoCommentCommand () { }

        public AddVideoCommentCommand (Guid videoId, string userId, string comment) {
            VideoId = videoId;
            UserId = userId;
            Comment = comment;
        }

        public AddVideoCommentCommand (long parentCommentId, string userId, string comment) {
            ParentCommentId = parentCommentId;
            UserId = userId;
            Comment = comment;
        }
    }
}
