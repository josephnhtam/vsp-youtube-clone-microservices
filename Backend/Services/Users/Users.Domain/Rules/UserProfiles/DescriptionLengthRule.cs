using Domain.Rules;

namespace Users.Domain.Rules.UserProfiles {
    public class DescriptionLengthRule : LengthRule {
        public const string PropertyName = "Description";
        public const int MaxLength = 1000;

        public DescriptionLengthRule (string description) : base(description, PropertyName, null, MaxLength) { }
    }
}
