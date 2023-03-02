using Domain.Rules;

namespace Users.Domain.Rules.UserProfiles {
    public class EmailPatternRule : RegexMatchingRule {
        private const string Pattern = "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
        public EmailPatternRule (string? text) : base(text, "Email", Pattern) {
        }
    }
}
