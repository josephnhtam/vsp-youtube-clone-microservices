using Domain.Rules;

namespace Users.Domain.Rules.UserProfiles {
    public class EmailLengthRule : LengthRule {
        public EmailLengthRule (string? email) : base(email, "Email", null, 255) { }
    }
}
