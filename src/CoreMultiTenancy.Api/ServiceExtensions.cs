using System.Reflection;
using Cmt.Protobuf;
using CoreMultiTenancy.Api.Data;
using CoreMultiTenancy.Api.Interfaces;
using CoreMultiTenancy.Api.Tenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CoreMultiTenancy.Api;

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

        builder.Services.AddScoped<ITenantProvider, RouteDataTenantProvider>();
        builder.Services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
        builder.Services.AddGrpc();
        builder.Services.AddGrpcClients();

        return builder.Build();
    }

    public static void AddGrpcClients(this IServiceCollection sc)
    {
        sc.AddGrpcClient<PermissionAuthorize.PermissionAuthorizeClient>(o =>
        {
            o.Address = new Uri("https://localhost:5100");
        });
    }
}