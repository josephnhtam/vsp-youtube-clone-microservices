using Application.Contracts;

namespace VideoManager.API.Application.Commands {
    public class SetVideoPublishStatusCommand : ICommand {
        public Guid VideoId { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset Date { get; set; }
        public long Version { get; set; }

        public SetVideoPublishStatusCommand (Guid videoId, bool isPublished, DateTimeOffset date, long version) {
            VideoId = videoId;
            IsPublished = isPublished;
            Date = date;
            Version = version;
        }
    }
}
