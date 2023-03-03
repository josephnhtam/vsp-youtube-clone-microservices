using Domain.Rules;

namespace Library.Domain.Rules.Playlists {
    public class DescriptionLengthRule : LengthRule {
        public const string PropertyName = "Description";
        public const int MaxLength = 5000;

        public DescriptionLengthRule (string text) : base(text, PropertyName, null, MaxLength) { }
    }
}
