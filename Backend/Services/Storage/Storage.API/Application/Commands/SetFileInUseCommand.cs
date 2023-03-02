using Application.Contracts;

namespace Storage.API.Application.Commands {
    public class SetFileInUseCommand : ICommand {
        public List<Guid> FileIds { get; set; }

        public SetFileInUseCommand (List<Guid> fileIds) {
            FileIds = fileIds;
        }
    }
}
