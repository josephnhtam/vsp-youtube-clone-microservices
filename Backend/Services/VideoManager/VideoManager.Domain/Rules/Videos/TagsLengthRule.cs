using Domain.Rules;

namespace VideoManager.Domain.Rules.Videos {
    public class TagsLengthRule : LengthRule {
        public TagsLengthRule (string text) : base(text, "Tags", null, 500) { }
    }
}
