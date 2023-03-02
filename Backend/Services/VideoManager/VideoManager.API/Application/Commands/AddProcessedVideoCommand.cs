using Application.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands {
    public class AddProcessedVideoCommand : ICommand {
        public Guid VideoId { get; set; }
        public ProcessedVideo Video { get; set; }

        public AddProcessedVideoCommand (Guid videoId, ProcessedVideo video) {
            VideoId = videoId;
            Video = video;
        }
    }
}
