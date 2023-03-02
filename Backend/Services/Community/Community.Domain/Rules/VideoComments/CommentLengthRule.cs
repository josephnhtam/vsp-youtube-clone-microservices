using Domain.Rules;

namespace Community.Domain.Rules.VideoComments {
    public class CommentLengthRule : LengthRule {
        public CommentLengthRule (string text) : base(text, "Comment", 1, 5000) { }
    }
}
