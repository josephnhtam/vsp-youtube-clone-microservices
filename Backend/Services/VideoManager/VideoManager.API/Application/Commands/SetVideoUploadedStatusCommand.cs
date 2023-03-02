using Application.Contracts;

namespace VideoManager.API.Application.Commands {
    public class SetVideoUploadedStatusCommand : ICommand {
        public Guid VideoId { get; set; }
        public string CreatorId { get; set; }
        public string OriginalFileName { get; set; }
        public string Url { get; set; }
        public DateTimeOffset Date { get; set; }

        public SetVideoUploadedStatusCommand (Guid videoId, string creatorId, string originalFileName, string url, DateTimeOffset date) {
            VideoId = videoId;
            CreatorId = creatorId;
            OriginalFileName = originalFileName;
            Url = url;
            Date = date;
        }
    }
}
