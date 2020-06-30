using System;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CoreMultiTenancy.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCustomAuthentication(Configuration);
            services.AddAuthorization(o =>
            {
                o.AddPolicy("ApiScope", p =>
                {
                    p.RequireAuthenticatedUser();
                    p.RequireClaim("scope", "testapi", "tid");
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization("ApiScope");
            });
        }
    }
    static class Extensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var identityUrl = config.GetValue<string>("IdentityUrl");
            var callbackUrl = config.GetValue<string>("CallbackUrl");
            var cookieLifetime = config.GetValue("CookieLifetime", 60);

            IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
                {
                    o.Authority = identityUrl;
                    o.RequireHttpsMetadata = false; // NOTE: dev only
                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = false,
                    };
                });

            // services.AddAuthentication(options =>
            // {
            //     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            // }).AddJwtBearer(options =>
            // {
            //     options.Authority = identityUrl;
            //     options.RequireHttpsMetadata = false; // NOTE: only for development
            //     options.Audience = "totalflightapi";
            // }).AddCookie(setup =>
            // {
            //     setup.ExpireTimeSpan = TimeSpan.FromMinutes(cookieLifetime);
            // }).AddOpenIdConnect(options =>
            // {
            //     options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //     options.Authority = identityUrl;
            //     options.SignedOutRedirectUri = callbackUrl;
            //     options.ClientId = "totalflightapi";
            //     options.ClientSecret = "temporary";
            //     options.ResponseType = "code id_token";
            //     options.SaveTokens = true;
            //     options.GetClaimsFromUserInfoEndpoint = true;
            //     options.RequireHttpsMetadata = false; // NOTE: only for development
            // });

            return services;
        }
    }
}
