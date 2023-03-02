using Application.Contracts;

namespace Community.API.Application.Commands {
    public class UnregisterVideoForumCommand : ICommand {
        public Guid VideoId { get; set; }

        public UnregisterVideoForumCommand (Guid videoId) {
            VideoId = videoId;
        }
    }
}
