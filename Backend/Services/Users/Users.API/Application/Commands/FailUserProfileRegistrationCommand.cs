using Application.Contracts;

namespace Users.API.Application.Commands {
    public class CompleteUserProfileRegistrationCommand : ICommand {
        public string UserId { get; set; }

        public CompleteUserProfileRegistrationCommand (string userId) {
            UserId = userId;
        }
    }
}
