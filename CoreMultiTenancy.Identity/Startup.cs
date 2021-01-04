using System;
using CoreMultiTenancy.Identity.Data;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Options;
using CoreMultiTenancy.Identity.Services;
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
using CoreMultiTenancy.Identity.Grpc.Aircraft;
using Microsoft.AspNetCore.Routing;

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

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<UserSignInManager>()
                .AddDefaultTokenProviders();
                
            services.AddApiVersioning();
            services.AddAuthentication();

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
            // Repositories
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserOrganizationRepository, UserOrganizationRepository>();
            services.AddScoped<IUserOrganizationRoleRepository, UserOrganizationRoleRepository>();

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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.SynchronizePermissionsToDb();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseNotFoundFilter("/error/notfound");
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(e =>
            {
                e.MapRazorPages();
                e.MapGrpcAuthorizationServices();
            });
        }
    }
    public static class StartupExtensions
    {
        public static IApplicationBuilder UseNotFoundFilter(this IApplicationBuilder app, string path)
        {
            return app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
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
            e.MapGrpcService<BaseAuthzService>();
            e.MapGrpcService<CreateAircraftService>();
            e.MapGrpcService<EditAircraftService>();
            return e;
        }

        /// <summary>
        /// Synchronously seeds database with Permissions and PermissionCategories from enum values.
        /// </summary>
        public static IApplicationBuilder SynchronizePermissionsToDb(this IApplicationBuilder builder)
        {
            // TODO: Push this out to a script, migration code, or publish profile
            using (var serviceScope = builder.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.SynchronizePermissions();
            }
            return builder;
        }
    }
}
