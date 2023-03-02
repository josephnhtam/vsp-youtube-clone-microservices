using Domain.Rules;

namespace Users.Domain.Rules.UserProfiles {
    public class HandleLengthRule : LengthRule {
        public HandleLengthRule (string? handle) : base(handle, "Handle", null, 30) { }
    }
}
