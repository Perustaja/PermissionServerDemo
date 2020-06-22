// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using CoreMultiTenancy.Identity.Data;
using CoreMultiTenancy.Identity.Models;
using CoreMultiTenancy.Identity.Services;
using CoreMultiTenancy.Identity.Tenancy;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace CoreMultiTenancy.Identity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("IdentityDb")));

            services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<UserSignInManager>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                // Support multi-step login
                .AddCookie(Constants.PartialLoginScheme);

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestData.TestUsers.Users)
                .AddAspNetIdentity<User>()
                .AddProfileService<TenantedProfileService>();
            builder.AddDeveloperSigningCredential();

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication(); // May be unnecessary
            app.UseIdentityServer();

            app.UseEndpoints(e =>
                {
                    e.MapRazorPages();
                });
        }
    }
}
