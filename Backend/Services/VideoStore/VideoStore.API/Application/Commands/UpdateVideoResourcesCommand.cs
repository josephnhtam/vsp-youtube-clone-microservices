using Application.Contracts;
using VideoStore.Domain.Models;

namespace VideoStore.API.Application.Commands {
    public class UpdateVideoResourcesCommand : ICommand {
        public Guid VideoId { get; set; }
        public List<ProcessedVideo> Videos { get; set; }
        public bool Merge { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public UpdateVideoResourcesCommand (Guid videoId, List<ProcessedVideo> videos, bool merge, DateTimeOffset updateDate) {
            VideoId = videoId;
            Videos = videos;
            Merge = merge;
            UpdateDate = updateDate;
        }
    }
}
