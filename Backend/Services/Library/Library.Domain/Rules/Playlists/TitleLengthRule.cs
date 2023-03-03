using Domain.Rules;

namespace Library.Domain.Rules.Playlists {
    public class TitleLengthRule : LengthRule {
        public const string PropertyName = "Title";
        public const int MinLength = 1;
        public const int MaxLength = 50;

        public TitleLengthRule (string text) : base(text, PropertyName, MinLength, MaxLength) { }
    }
}
