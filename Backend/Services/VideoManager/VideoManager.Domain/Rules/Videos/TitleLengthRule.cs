using Domain.Rules;

namespace VideoManager.Domain.Rules.Videos {
    public class TitleLengthRule : LengthRule {
        public const string PropertyName = "Title";
        public const int MinLength = 1;
        public const int MaxLength = 50;

        public TitleLengthRule (string text) : base(text, PropertyName, MinLength, MaxLength) { }
    }
}
