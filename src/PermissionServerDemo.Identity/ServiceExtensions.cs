using System.Reflection;
using PermissionServerDemo.Core.Authorization;
using PermissionServerDemo.Identity.Authorization;
using PermissionServerDemo.Identity.Data;
using PermissionServerDemo.Identity.Data.Configuration.DependencyInjection;
using PermissionServerDemo.Identity.Data.Repositories;
using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Identity.Entities.Dtos;
using PermissionServerDemo.Identity.Extensions;
using PermissionServerDemo.Identity.Interfaces;
using PermissionServerDemo.Identity.Mapping;
using PermissionServerDemo.Identity.Options;
using PermissionServerDemo.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PermissionServer;

namespace PermissionServerDemo.Identity;

internal static class ServiceExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>();

        builder.Services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager<UserSignInManager>()
            .AddDefaultTokenProviders();

        builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("https://localhost:44459")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryIdentityResources(Config.Ids)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddAspNetIdentity<User>();

        builder.Services.AddHttpContextAccessor();

        // Services
        builder.Services.Configure<EmailSenderOptions>(builder.Configuration.GetSection("Email"));
        builder.Services.Configure<OidcAccountOptions>(builder.Configuration.GetSection("OidcAccountOptions"));
        builder.Services.AddScoped<IEmailSender, EmailSender>();
        builder.Services.AddScoped<IOrganizationManager, OrganizationManager>();
        builder.Services.AddScoped<IOrganizationInviteService, OrganizationInviteService>();
        builder.Services.AddScoped<IAccountEmailService, AccountEmailService>();
        builder.Services.AddScoped<IPermissionService, PermissionService>();
        // Repositories
        builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();
        builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        builder.Services.AddScoped<IUserOrganizationRepository, UserOrganizationRepository>();
        builder.Services.AddScoped<IUserOrganizationRoleRepository, UserOrganizationRoleRepository>();

        builder.Services.AddRazorPages()
            .AddRazorPagesOptions(o =>
            {
                // Add default home page
                o.Conventions.AddPageRoute("/home/index", "");
            })
            .AddRazorRuntimeCompilation();


        // Identity config
        builder.Services.Configure<IdentityOptions>(options =>
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
        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => false;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        builder.Services.Configure<CookieTempDataProviderOptions>(options =>
        {
            options.Cookie.Name = "CMTApp";
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddPermissionServer<PermissionEnum, PermissionCategoryEnum>(o =>
            {
                o.RouteDataTenantIdentifier = "orgId";
            })
            .AddRemoteAuthorization("https://localhost:6100", true)
            .AddAuthorizationEvaluator<AuthorizationEvaluator>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddLocalApiAuthentication();
        builder.Services.AddAutoMapperWithTypeConverters();
        builder.AddDemoGlobalRoles();

        return builder.Build();
    }

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

    private static void AddAutoMapperWithTypeConverters(this IServiceCollection sc)
    {
        sc.AddTransient<RolePermissionConverter>();

        sc.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(Assembly.GetExecutingAssembly());
            cfg.CreateMap<RolePermission, PermissionGetDto>()
                .ConvertUsing<RolePermissionConverter>();
        });
    }

    private static void AddDemoGlobalRoles(this WebApplicationBuilder builder)
    {
        builder.Services.AddGlobalRoles(options =>
        {
            // in a normal application you will not need to hardcode these ids, the demo
            // just seeds some users for brevity
            var adminRoleId = builder.Configuration.GetDemoRoleId("DefaultAdminRoleId");
            var newUserRoleId = builder.Configuration.GetDemoRoleId("DefaultNewUserRoleId");
            var aircraftCreateRoleId = builder.Configuration.GetDemoRoleId("AircraftCreateRoleId");

            options.AddGlobalRole(role =>
            {
                role.WithBaseRoleForDemo(adminRoleId, "Owner", "Default admin role for new tenant owners")
                    .AsDefaultAdminRole()
                    .GrantAllPermissionsExcept(PermissionEnum.AircraftCreate);
            });
            options.AddGlobalRole(role =>
            {
                role.WithBaseRoleForDemo(newUserRoleId, "User", "Default user role with minimal permissions")
                    .AsDefaultNewUserRole();
            });
            options.AddGlobalRole(role =>
            {
                role.WithBaseRoleForDemo(aircraftCreateRoleId, "Create Aircraft", "Role for creating new aircraft")
                    .GrantPermissions(PermissionEnum.AircraftCreate);
            });
        });
    }
}