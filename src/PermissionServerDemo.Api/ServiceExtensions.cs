using System.Reflection;
using PermissionServerDemo.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PermissionServer;
using PermissionServerDemo.Core.Authorization;

namespace PermissionServerDemo.Api;

internal static class ServiceExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<TenantedDbContext>();

        builder.Services.AddControllers();
        builder.Services.AddApiVersioning();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Bearer", o =>
            {
                o.MapInboundClaims = false;
                o.Authority = "https://localhost:5100";
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                };
            });

        builder.Services.AddAuthorization(o =>
        {
            o.AddPolicy("ApiScope", p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireClaim("scope", "testapi");
            });
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

        builder.Services.AddPermissionServer<PermissionEnum, PermissionCategoryEnum>()
            .AddRemoteAuthorization("https://localhost:5100");

        builder.Services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));

        return builder.Build();
    }
}