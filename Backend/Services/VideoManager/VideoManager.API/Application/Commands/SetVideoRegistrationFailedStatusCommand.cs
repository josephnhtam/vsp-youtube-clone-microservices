using Application.Contracts;

namespace VideoManager.API.Application.Commands {
    public class SetVideoRegistrationFailedStatusCommand : ICommand {
        public Guid VideoId { get; set; }

        public SetVideoRegistrationFailedStatusCommand (Guid videoId) {
            VideoId = videoId;
        }
    }
}
