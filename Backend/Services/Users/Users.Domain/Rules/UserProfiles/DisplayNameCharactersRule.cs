using Domain.Rules;

namespace Users.Domain.Rules.UserProfiles {
    public class DisplayNameCharactersRule : CharactersRule {
        private readonly static char[] InvalidCharacters = new char[] { '<', '>' };
        public DisplayNameCharactersRule (string? text) : base(text, "Display name", InvalidCharacters) { }
    }
}
