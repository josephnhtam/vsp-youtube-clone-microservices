using Application.Validations.Extensions;
using FluentValidation;
using Users.Domain.Rules.UserProfiles;

namespace Users.API.Application.Commands.Validators {
    public class CreateUserProfileCommandValidator : AbstractValidator<CreateUserProfileCommand> {
        public CreateUserProfileCommandValidator () {
            RuleFor(x => x.DisplayName).AdhereRule(displayName => new DisplayNameLengthRule(displayName));
            RuleFor(x => x.DisplayName).AdhereRule(displayName => new DisplayNameCharactersRule(displayName));
        }
    }
}
