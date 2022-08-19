using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace CoreMultiTenancy.Identity
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
                new ApiScope("testapi", "Test API")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            { 
                new Client()
                {
                    ClientName = "Test Angular Client",
                    ClientId = "testclient",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedCorsOrigins = { "https://localhost:44459" },
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = { "https://localhost:44459/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44459/signout-callback-oidc" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "testapi",
                    },
                    ClientSecrets = { new Secret("secret".Sha256()) },
                },
            };

    }
}