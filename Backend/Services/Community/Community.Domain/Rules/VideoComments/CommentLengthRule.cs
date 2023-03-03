using Domain.Rules;

namespace Community.Domain.Rules.VideoComments {
    public class CommentLengthRule : LengthRule {
        public const string PropertyName = "Comment";
        public const int MinLength = 1;
        public const int MaxLength = 5000;

        public CommentLengthRule (string text) : base(text, PropertyName, MinLength, MaxLength) { }
    }
}
