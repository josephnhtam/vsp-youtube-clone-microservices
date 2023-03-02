using Domain.Rules;

namespace Library.Domain.Rules.Playlists {
    public class TitleLengthRule : LengthRule {
        public TitleLengthRule (string text) : base(text, "Title", 1, 50) { }
    }
}
