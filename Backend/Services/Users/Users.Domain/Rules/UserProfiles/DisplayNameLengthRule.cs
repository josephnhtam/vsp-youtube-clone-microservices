using Domain.Rules;

namespace Users.Domain.Rules.UserProfiles {
    public class DisplayNameLengthRule : LengthRule {
        public DisplayNameLengthRule (string displayName) : base(displayName, "Display name", 1, 50) { }
    }
}
