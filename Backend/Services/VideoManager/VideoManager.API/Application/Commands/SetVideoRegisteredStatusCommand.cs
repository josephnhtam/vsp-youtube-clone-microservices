
using Application.Contracts;

namespace VideoManager.API.Application.Commands {
    public class SetVideoRegisteredStatusCommand : ICommand {
        public Guid VideoId { get; set; }

        public SetVideoRegisteredStatusCommand (Guid videoId) {
            VideoId = videoId;
        }
    }
}
