using Application.Contracts;

namespace History.API.Application.Commands {
    public class RecordVideoViewCommand : ICommand {
        public string? UserId { get; set; }
        public Guid VideoId { get; set; }

        public RecordVideoViewCommand (string? userId, Guid videoId) {
            UserId = userId;
            VideoId = videoId;
        }
    }
}
