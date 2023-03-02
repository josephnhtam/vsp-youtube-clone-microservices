using Application.Contracts;

namespace Storage.API.Application.Commands {
    public class RemoveFileCommand : ICommand {
        public List<Guid> FileIds { get; set; }
        public TimeSpan? RemovalDelay { get; set; }

        public RemoveFileCommand (List<Guid> fileIds, TimeSpan? removalDelay) {
            FileIds = fileIds;
            RemovalDelay = removalDelay;
        }
    }
}
