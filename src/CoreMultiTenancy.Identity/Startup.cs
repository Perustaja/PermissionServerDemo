﻿using System;
using CoreMultiTenancy.Identity.Data;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Options;
using CoreMultiTenancy.Identity.Services;
using CoreMultiTenancy.Identity.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CoreMultiTenancy.Identity.Grpc;
using Microsoft.AspNetCore.Routing;
using Cmt.Protobuf;
using System.Reflection;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

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
                options.UseSqlite(Configuration.GetConnectionString("IdentityDb")));

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<UserSignInManager>()
                .AddDefaultTokenProviders();

            services.AddApiVersioning();

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddAspNetIdentity<User>();

            builder.AddDeveloperSigningCredential();

            services.AddGrpc();
            services.AddHttpContextAccessor();

            // Services
            services.Configure<EmailSenderOptions>(Configuration.GetSection("Email"));
            services.Configure<OidcAccountOptions>(Configuration.GetSection("OidcAccountOptions"));
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IOrganizationManager, OrganizationManager>();
            services.AddScoped<IOrganizationInviteService, OrganizationInviteService>();
            services.AddScoped<IAccountEmailService, AccountEmailService>();
            services.AddScoped<IRemoteAuthorizationEvaluator, RemoteAuthorizationEvaluator>();
            services.AddScoped<IPermissionService, PermissionService>();
            // Repositories
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserOrganizationRepository, UserOrganizationRepository>();
            services.AddScoped<IUserOrganizationRoleRepository, UserOrganizationRoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();

            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));

            services.AddRazorPages()
                .AddRazorPagesOptions(o =>
                {
                    // Add default home page
                    o.Conventions.AddPageRoute("/home/index", "");
                })
                .AddRazorRuntimeCompilation();


            // Identity config
            services.Configure<IdentityOptions>(options =>
            {
                // Lockout
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                // Password
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            });

            // Cookie config
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.Name = "CMTApp";
                options.Cookie.IsEssential = true;
            });

            services.AddGrpcClients();
            services.AddLocalApiAuthentication();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/api/error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRazorPagesNotFoundFilter("/error/notfound");

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(e =>
            {
                e.MapRazorPages();
                e.MapControllers();
                e.MapGrpcAuthorizationServices();
            });
        }
    }

    public static class StartupExtensions
    {
        // Ensures that Not Found razor page results are not sent as api responses
        public static IApplicationBuilder UseRazorPagesNotFoundFilter(this IApplicationBuilder app, string path)
        {
            return app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404 && !context.Response.HasStarted
                    && !context.Request.Path.Value.Contains("api"))
                {
                    // If 404 response, re-execute with notfound path request,
                    // this proliferates the existing url in the user's browser.
                    context.Request.Path = path;
                    await next();
                }
            });
        }

        public static IEndpointRouteBuilder MapGrpcAuthorizationServices(this IEndpointRouteBuilder e)
        {
            e.MapGrpcService<PermissionAuthorizeService>();
            return e;
        }

        public static void AddGrpcClients(this IServiceCollection sc)
        {
            sc.AddGrpcClient<PermissionAuthorize.PermissionAuthorizeBase>(o =>
            {
                o.Address = new Uri("https://localhost:6100");
            });
        }
    }
}