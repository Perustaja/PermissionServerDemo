using System;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            // services.AddCustomAuthentication(Configuration);
            services.AddControllers();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:5100/";
                    options.RequireHttpsMetadata = false; // NOTE: dev only
                    options.ApiName = "totalflightapi";
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    static class Extensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration config)
        {
            // The authority url (CoreMultiTenancy.Identity)
            var identityUrl = config.GetValue<string>("IdentityUrl");
            var callbackUrl = config.GetValue<string>("CallbackUrl");
            var cookieLifetime = config.GetValue("CookieLifetime", 60);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false; // NOTE: only for development
                options.Audience = "totalflightapi";
            }).AddCookie(setup =>
            {
                setup.ExpireTimeSpan = TimeSpan.FromMinutes(cookieLifetime);
            }).AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = identityUrl;
                options.SignedOutRedirectUri = callbackUrl;
                options.ClientId = "totalflightapi";
                options.ClientSecret = "temporary";
                options.ResponseType = "code id_token";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = false; // NOTE: only for development
            });

            return services;
        }
    }
}
