using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityProvider.Pages.Logout;

[SecurityHeaders]
[AllowAnonymous]
public class LoggedOut : PageModel {
    private readonly IIdentityServerInteractionService _interactionService;
    private readonly SignInManager<ApplicationUser> _signinManager;

    public LoggedOutViewModel View { get; set; }

    public LoggedOut (IIdentityServerInteractionService interactionService, SignInManager<ApplicationUser> signinManager) {
        _interactionService = interactionService;
        _signinManager = signinManager;
    }

    public async Task OnGet (string logoutId) {
        await _signinManager.SignOutAsync();

        // get context information (client name, post logout redirect URI and iframe for federated signout)
        var logout = await _interactionService.GetLogoutContextAsync(logoutId);

        View = new LoggedOutViewModel {
            AutomaticRedirectAfterSignOut = LogoutOptions.AutomaticRedirectAfterSignOut,
            PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
            ClientName = String.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
            SignOutIframeUrl = logout?.SignOutIFrameUrl
        };
    }
}