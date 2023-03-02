using Domain.Rules;

namespace Users.Domain.Rules.UserProfiles {
    public class DescriptionLengthRule : LengthRule {
        public DescriptionLengthRule (string description) : base(description, "Description", null, 1000) { }
    }
}
