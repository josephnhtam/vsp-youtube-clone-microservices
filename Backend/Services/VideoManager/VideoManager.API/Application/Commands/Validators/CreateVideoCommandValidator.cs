using Application.Validations.Extensions;
using FluentValidation;
using VideoManager.API.Commands;
using VideoManager.Domain.Rules.Videos;

namespace VideoManager.API.Application.Commands.Validators {
    public class CreateVideoCommandValidator : AbstractValidator<CreateVideoCommand> {
        public CreateVideoCommandValidator () {
            RuleFor(x => x.Title).AdhereRule(title => new TitleLengthRule(title));
            RuleFor(x => x.Description).AdhereRule(description => new DescriptionLengthRule(description));
        }
    }
}
