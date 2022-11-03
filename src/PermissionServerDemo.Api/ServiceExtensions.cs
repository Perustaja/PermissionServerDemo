using System.Reflection;
using Psd.Protobuf;
using PermissionServerDemo.Api.Data;
using PermissionServerDemo.Api.Tenancy;
using PermissionServerDemo.Core.Tenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Grpc.Net.Client.Web;

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
                o.Authority = "https://idp.permissionserverdemo.dev";
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
                policy.WithOrigins("https://permissionserverdemo.dev")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
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
        sc.AddGrpcClient<GrpcPermissionAuthorize.GrpcPermissionAuthorizeClient>(o =>
        {
            o.Address = new Uri("https://permissionserverdemo.dev");
        })
        .ConfigurePrimaryHttpMessageHandler(
            () => new GrpcWebHandler(new HttpClientHandler()));
    }
}