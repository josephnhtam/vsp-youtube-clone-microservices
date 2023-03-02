using Microsoft.AspNetCore.Identity;

namespace IdentityProvider {
    public class ApplicationUser : IdentityUser {

        public string Nickname { get; set; }

    }
}
