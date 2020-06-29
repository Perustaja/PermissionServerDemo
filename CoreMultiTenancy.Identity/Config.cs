// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


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
                // The current selected tenant
                new IdentityResource("tid", "Tenant Id", new string[] { "tid" }),
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("testapi", "Test API", new string[] { "tid" }),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            { 
                // MVC
                new Client()
                {
                    ClientName = "Test MVC Client",
                    ClientId = "testmvc",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    RequireConsent = false,
                    // RequirePkce = true,
                    // RedirectUris = { "https://localhost:5001/signin-oidc" },
                    // PostLogoutRedirectUris = { "https://localhost:5001/signout-callback-oidc" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "tid",
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
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "tid",
                        "testapi",
                    },
                    ClientSecrets = { new Secret("secret".Sha256()) },
                },
            };

    }
}