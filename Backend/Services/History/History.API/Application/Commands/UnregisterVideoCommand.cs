using Application.Contracts;

namespace History.API.Application.Commands {
    public class UnregisterVideoCommand : ICommand {
        public Guid VideoId { get; set; }

        public UnregisterVideoCommand (Guid videoId) {
            VideoId = videoId;
        }
    }
}
