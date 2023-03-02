using Application.Contracts;

namespace VideoManager.API.Application.Commands {
    public class UnregisterVideoCommand : ICommand {
        public string UserId { get; set; }
        public Guid VideoId { get; set; }

        public UnregisterVideoCommand (string userId, Guid videoId) {
            UserId = userId;
            VideoId = videoId;
        }
    }
}
