using Application.Contracts;

namespace Users.API.Application.Commands {
    public class FailUserProfileRegistrationCommand : ICommand {
        public string UserId { get; set; }

        public FailUserProfileRegistrationCommand (string userId) {
            UserId = userId;
        }
    }
}
