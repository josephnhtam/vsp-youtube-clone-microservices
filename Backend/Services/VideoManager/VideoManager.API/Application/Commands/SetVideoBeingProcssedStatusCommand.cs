using Application.Contracts;

namespace VideoManager.API.Application.Commands {
    public class SetVideoBeingProcssedStatusCommand : ICommand {
        public Guid VideoId { get; set; }

        public SetVideoBeingProcssedStatusCommand (Guid videoId) {
            VideoId = videoId;
        }
    }
}
