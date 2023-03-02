using Application.Validations.Extensions;
using FluentValidation;
using Users.API.Application.DtoModels;
using Users.Domain.Rules.UserChannels;
using Users.Domain.Rules.UserProfiles;

namespace Users.API.Application.Commands.Validators {
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand> {
        public UpdateUserCommandValidator () {
            RuleFor(x => x.UpdateBasicInfo!)
                .SetValidator(new UpdateBasicInfoValidator())
                .When(x => x.UpdateBasicInfo != null);

            RuleFor(x => x.UpdateLayout!)
                .SetValidator(new UpdateLayoutValidator())
                .When(x => x.UpdateLayout != null);
        }

        public class UpdateBasicInfoValidator : AbstractValidator<UpdateUserProfileBasicInfo> {
            public UpdateBasicInfoValidator () {
                RuleFor(x => x.DisplayName).AdhereRule(displayName => new DisplayNameLengthRule(displayName));
                RuleFor(x => x.DisplayName).AdhereRule(displayName => new DisplayNameCharactersRule(displayName));

                RuleFor(x => x.Description).AdhereRule(description => new DescriptionLengthRule(description));

                RuleFor(x => x.Handle).AdhereRule(handle => new HandleLengthRule(handle));
                RuleFor(x => x.Handle).AdhereRule(handle => new HandlePatternRule(handle));

                RuleFor(x => x.Email).AdhereRule(email => new EmailLengthRule(email));
                RuleFor(x => x.Email).AdhereRule(email => new EmailPatternRule(email));
            }
        }

        public class UpdateLayoutValidator : AbstractValidator<UpdateUserChannelLayout> {
            public UpdateLayoutValidator () {
                RuleFor(x => x.ChannelSections).AdhereRule(sections => new ChannelSectionsCountRule(sections.Count));
            }
        }
    }
}
