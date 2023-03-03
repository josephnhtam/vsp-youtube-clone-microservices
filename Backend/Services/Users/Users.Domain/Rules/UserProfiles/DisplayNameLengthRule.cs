using Domain.Rules;

namespace Users.Domain.Rules.UserProfiles {
    public class DisplayNameLengthRule : LengthRule {
        public const string PropertyName = "Display name";
        public const int MinLength = 1;
        public const int MaxLength = 50;

        public DisplayNameLengthRule (string displayName) : base(displayName, PropertyName, MinLength, MaxLength) { }
    }
}
