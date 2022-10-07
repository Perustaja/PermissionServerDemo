using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace PermissionServerDemo.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("testapi", "Test API"),
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)

            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client()
                {
                    // Angular SPA, Code with PKCE flow. Read links for information on why not to use implicit
                    // https://docs.identityserver.io/en/latest/topics/grant_types.html
                    // https://pragmaticwebsecurity.com/articles/oauthoidc/from-implicit-to-pkce.html
                    ClientName = "Test Angular Client",
                    ClientId = "testclient",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedCorsOrigins = { "https://localhost:44459" },
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = { "https://localhost:44459/authentication/login-callback" },
                    PostLogoutRedirectUris = { "https://localhost:44459/authentication/logout-callback" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.LocalApi.ScopeName,
                        "testapi",
                    },
                    // NOTE: Configure a client secret for production.
                    RequirePkce = true,
                    RequireClientSecret = false
                },
            };
    }
}