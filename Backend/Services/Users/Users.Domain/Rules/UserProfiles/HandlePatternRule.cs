using Domain.Rules;

namespace Users.Domain.Rules.UserProfiles {
    public class HandlePatternRule : RegexMatchingRule {
        private const string Pattern = "^[a-zA-Z 0-9\\.\\,\\+\\-]*$";
        public HandlePatternRule (string? text) : base(text, "Handle", Pattern) {
        }
    }
}
