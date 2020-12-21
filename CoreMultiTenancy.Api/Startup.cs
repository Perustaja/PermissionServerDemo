using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services.AddHttpContextAccessor();
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", o =>
                {
                    o.Authority = "https://localhost:5100";
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                    };
                });
            services.AddAuthorization(o =>
            {
                o.AddPolicy("ApiScope", p =>
                {
                    p.RequireAuthenticatedUser();
                    p.RequireClaim("scope", "testapi");
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
}
