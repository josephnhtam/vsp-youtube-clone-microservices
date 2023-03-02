using Application.Validations.Extensions;
using FluentValidation;
using Library.Domain.Rules.Playlists;

namespace Library.API.Application.Commands.Validators {
    public class UpdatePlaylistCommandValidator : AbstractValidator<UpdatePlaylistCommand> {
        public UpdatePlaylistCommandValidator () {
            RuleFor(x => x.Title!)
                .AdhereRule(title => new TitleLengthRule(title))
                .When(x => x.Title != null);

            RuleFor(x => x.Description!)
                .AdhereRule(description => new DescriptionLengthRule(description))
                .When(x => x.Description != null);

            RuleFor(x => x.Visibility)
                .AdhereRule(visibility => new ValidPlaylistVisibilityRule(visibility!.Value))
                .When(x => x.Visibility.HasValue);
        }
    }
}
