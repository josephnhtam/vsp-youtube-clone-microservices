using Application.Contracts;

namespace Library.API.Application.Commands {
    public class DeleteUserProfileCommand : ICommand {
        public string Id { get; set; }

        public DeleteUserProfileCommand (string id) {
            Id = id;
        }
    }
}
