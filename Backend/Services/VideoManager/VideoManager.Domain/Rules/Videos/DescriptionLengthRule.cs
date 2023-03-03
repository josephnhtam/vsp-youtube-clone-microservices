using Domain.Rules;

namespace VideoManager.Domain.Rules.Videos {
    public class DescriptionLengthRule : LengthRule {
        public const string PropertyName = "Description";
        public const int MaxLength = 5000;

        public DescriptionLengthRule (string text) : base(text, PropertyName, null, MaxLength) { }
    }
}
