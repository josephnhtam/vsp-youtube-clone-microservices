using Domain.Rules;

namespace VideoManager.Domain.Rules.Videos {
    public class DescriptionLengthRule : LengthRule {
        public DescriptionLengthRule (string text) : base(text, "Description", null, 5000) { }
    }
}
