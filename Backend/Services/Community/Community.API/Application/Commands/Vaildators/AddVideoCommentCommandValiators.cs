using Application.Validations.Extensions;
using Community.Domain.Rules.VideoComments;
using FluentValidation;

namespace Community.API.Application.Commands.Vaildators {
    public class AddVideoCommentCommandValiators : AbstractValidator<AddVideoCommentCommand> {
        public AddVideoCommentCommandValiators () {
            RuleFor(x => x.Comment).AdhereRule((comment) => new CommentLengthRule(comment));
        }
    }
}
