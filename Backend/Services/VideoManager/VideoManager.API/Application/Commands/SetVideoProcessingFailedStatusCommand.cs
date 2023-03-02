using Application.Contracts;

namespace VideoManager.API.Application.Commands {
    public class SetVideoProcessingFailedStatusCommand : ICommand {
        public Guid VideoId { get; set; }

        public SetVideoProcessingFailedStatusCommand (Guid videoId) {
            VideoId = videoId;
        }
    }
}
