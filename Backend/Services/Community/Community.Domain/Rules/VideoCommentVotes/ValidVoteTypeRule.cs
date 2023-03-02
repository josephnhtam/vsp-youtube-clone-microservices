using Community.Domain.Models;
using Domain.Rules;

namespace Community.Domain.Rules.VideoCommentVotes {
    public class ValidVoteTypeRule : DefinedEnumRule<VoteType> {
        public ValidVoteTypeRule (VoteType? voteType) : base(voteType, "Vote") { }
    }
}
