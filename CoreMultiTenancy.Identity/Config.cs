using IdentityModel;
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
                new IdentityResources.OpenId(), // Required for OpenID Connect
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
                // MVC
                new Client()
                {
                    ClientName = "Test MVC Client",
                    ClientId = "testmvc",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RedirectUris = { "https://localhost:5001/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5001/signout-callback-oidc" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "testapi",
                    },
                    ClientSecrets = { new Secret("secret".Sha256()) },
                },
                // Console (for basic testing via HttpClient)
                new Client()
                {
                    ClientName = "Test Console Client",
                    ClientId = "testconsole",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    RequireConsent = false,
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
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