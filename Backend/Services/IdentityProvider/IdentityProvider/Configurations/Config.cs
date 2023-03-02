using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace IdentityProvider.Configurations {
    public static class Config {

        public static IEnumerable<ApiScope> ApiScopes =>
             new List<ApiScope> {
                 new ApiScope("vsp_m2m_api"),
                 new ApiScope("vsp_api", new List<string>{ JwtClaimTypes.Name, JwtClaimTypes.Role })
             };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource> {
                new ApiResource("vsp_resource"){
                    Scopes = { "vsp_api", "vsp_m2m_api" }
                }
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", "User role(s)", new List<string>{ JwtClaimTypes.Role })
            };

        public static IEnumerable<Client> GetClients (IConfiguration configuration) =>
            new List<Client> {
                // Machine-to-machine client
                new Client{
                    ClientId = "m2m",
                    ClientSecrets = new List<Secret>{
                        new Secret(configuration.GetValue<string>("Secrets:m2m").Sha256())
                    },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = new List<string> {
                        IdentityServerConstants.StandardScopes.Profile,
                        "vsp_m2m_api",
                    }
                },

                // SPA client
                new Client {
                    ClientId = "spa",
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AccessTokenLifetime = 1800,
                    AbsoluteRefreshTokenLifetime = 2592000,
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowedGrantTypes = GrantTypes.Code,

                    AllowedScopes = new List<string> {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "roles",
                        "vsp_api"
                    },

                    RedirectUris = configuration.GetSection("ClientUrls:spa-signin").Get<string[]>(),
                    PostLogoutRedirectUris = configuration.GetSection("ClientUrls:spa-signout").Get<string[]>(),
                    AllowedCorsOrigins = configuration.GetSection("Cors:Origins").Get<string[]>()
                }
            };

    }
}
