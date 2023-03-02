using Domain.Rules;

namespace VideoManager.Domain.Rules.Videos {
    public class TitleLengthRule : LengthRule {
        public TitleLengthRule (string text) : base(text, "Title", 1, 100) { }
    }
}
