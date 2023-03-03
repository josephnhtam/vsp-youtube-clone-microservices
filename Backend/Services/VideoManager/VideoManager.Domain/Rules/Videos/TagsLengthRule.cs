using Domain.Rules;

namespace VideoManager.Domain.Rules.Videos {
    public class TagsLengthRule : LengthRule {
        public const string PropertyName = "Tags";
        public const int MaxLength = 500;

        public TagsLengthRule (string text) : base(text, PropertyName, null, MaxLength) { }
    }
}
