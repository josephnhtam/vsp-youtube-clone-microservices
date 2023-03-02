using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityProvider.Pages.Account.Register {
    public class IndexModel : PageModel {

        [BindProperty]
        public InputModel Input { get; set; }

        private readonly IEventService _events;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IIdentityProviderStore _identityProviderStore;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel (
            IEventService events,
            IIdentityServerInteractionService interaction,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signinManager,
            IAuthenticationSchemeProvider schemeProvider,
            IIdentityProviderStore identityProviderStore,
            ApplicationDbContext dbContext,
            ILogger<IndexModel> logger) {
            _events = events;
            _interaction = interaction;
            _userManager = userManager;
            _signinManager = signinManager;
            _schemeProvider = schemeProvider;
            _identityProviderStore = identityProviderStore;
            _dbContext = dbContext;
            _logger = logger;
        }

        public void OnGet (string returnUrl) {
            Input = new InputModel {
                ReturnUrl = returnUrl
            };
        }

        public IActionResult OnGetReturnToLogin (string returnUrl) {
            return RedirectToPage("/Account/Login/Index", new {
                returnUrl = returnUrl
            });
        }

        public async Task<IActionResult> OnPost () {
            if (Input.Password != Input.ConfirmPassword) {
                ModelState.AddModelError("Input.Password", RegisterOptions.PasswordConfirmErrorMessage);
                return Page();
            }

            var user = new ApplicationUser {
                UserName = Input.Username,
                Nickname = Input.Nickname,
                Email = Input.Email,
                EmailConfirmed = true,
            };

            try {
                await RegisterUser(user, Input.Password, Roles.User);
                return await Login(user);
            } catch (FailedToRegisterException ex) {
                foreach (var error in ex.Result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to regiseter user");
                ModelState.AddModelError(string.Empty, "Failed to register.");
                return Page();
            }
        }

        private async Task<IActionResult> Login (ApplicationUser user) {
            var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

            await _signinManager.SignInAsync(user, false);

            await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

            if (context != null) {
                if (context.IsNativeClient()) {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl);
                }

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return Redirect(Input.ReturnUrl);
            }

            // request for a local page
            if (Url.IsLocalUrl(Input.ReturnUrl)) {
                return Redirect(Input.ReturnUrl);
            } else if (string.IsNullOrEmpty(Input.ReturnUrl)) {
                return Redirect("~/");
            } else {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }
        }

        private async Task RegisterUser (ApplicationUser user, string password, string role) {
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () => {
                var createResult = await _userManager.CreateAsync(user, password);

                if (!createResult.Succeeded) {
                    throw new FailedToRegisterException(createResult);
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, role);

                if (!addRoleResult.Succeeded) {
                    throw new FailedToRegisterException(addRoleResult);
                }

                var addClaimResult = await _userManager.AddClaimsAsync(user, new Claim[] {
                        new Claim(JwtClaimTypes.Name, user.Nickname),
                    });

                if (!addClaimResult.Succeeded) {
                    throw new FailedToRegisterException(addClaimResult);
                }

                await _dbContext.SaveChangesAsync();
            });
        }

        private class FailedToRegisterException : Exception {
            public readonly IdentityResult Result;

            public FailedToRegisterException (IdentityResult result) {
                Result = result;
            }
        }

    }
}
