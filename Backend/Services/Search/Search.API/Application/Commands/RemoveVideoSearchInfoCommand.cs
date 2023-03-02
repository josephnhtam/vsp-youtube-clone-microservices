using Application.Contracts;

namespace Search.API.Application.Commands {
    public class RemoveVideoSearchInfoCommand : ICommand {
        public Guid VideoId { get; set; }
        public long Version { get; set; }

        public RemoveVideoSearchInfoCommand () { }

        public RemoveVideoSearchInfoCommand (Guid videoId, long version) {
            VideoId = videoId;
            Version = version;
        }
    }
}
