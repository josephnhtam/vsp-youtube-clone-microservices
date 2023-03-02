using Application.Validations.Extensions;
using FluentValidation;
using Library.Domain.Rules.Playlists;

namespace Library.API.Application.Commands.Validators {
    public class CreatePlaylistCommandValidator : AbstractValidator<CreatePlaylistCommand> {
        public CreatePlaylistCommandValidator () {
            RuleFor(x => x.Title).AdhereRule(title => new TitleLengthRule(title));
            RuleFor(x => x.Title).AdhereRule(title => new TitleLengthRule(title));
            RuleFor(x => x.Title).AdhereRule(title => new TitleLengthRule(title));
            RuleFor(x => x.Description).AdhereRule(description => new DescriptionLengthRule(description));
            RuleFor(x => x.Visibility).AdhereRule(visibility => new ValidPlaylistVisibilityRule(visibility));
        }
    }
}
