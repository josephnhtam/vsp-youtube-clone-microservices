using Application.Validations.Extensions;
using Community.Domain.Rules.VideoCommentVotes;
using FluentValidation;

namespace Community.API.Application.Commands.Vaildators {
    public class VoteVideoCommentCommandValidator : AbstractValidator<VoteVideoCommentCommand> {
        public VoteVideoCommentCommandValidator () {
            RuleFor(x => x.VoteType).AdhereRule(voteType => new ValidVoteTypeRule(voteType));
        }
    }
}
