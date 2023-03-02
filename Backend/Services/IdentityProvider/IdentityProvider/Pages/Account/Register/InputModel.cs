using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Account.Register {
    public class InputModel {
        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, MaxLength(50)]
        public string Nickname { get; set; }

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; }

        [Required, MaxLength(32)]
        public string Password { get; set; }

        [Required, MaxLength(32)]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }
    }
}
