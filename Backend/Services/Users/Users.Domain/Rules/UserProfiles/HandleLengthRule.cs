using Domain.Rules;

namespace Users.Domain.Rules.UserProfiles {
    public class HandleLengthRule : LengthRule {
        public const string PropertyName = "Handle";
        public const int MaxLength = 30;

        public HandleLengthRule (string? handle) : base(handle, PropertyName, null, MaxLength) { }
    }
}
