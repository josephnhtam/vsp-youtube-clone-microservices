using Domain.Rules;

namespace Library.Domain.Rules.Playlists {
    public class DescriptionLengthRule : LengthRule {
        public DescriptionLengthRule (string text) : base(text, "Description", null, 5000) { }
    }
}
