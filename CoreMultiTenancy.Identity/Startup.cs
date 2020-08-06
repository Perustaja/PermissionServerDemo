using System;
using CoreMultiTenancy.Identity.Data;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;
using CoreMultiTenancy.Identity.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<UserSignInManager>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                // Support multi-step login
                .AddCookie(Constants.Schemes.PortalLoginScheme);

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddAspNetIdentity<User>();
            builder.AddDeveloperSigningCredential();

            services.Configure<EmailSenderOptions>(Configuration.GetSection("Email"));
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IOrganizationAccessManager, OrganizationAccessManager>();
            services.AddScoped<IOrganizationInviteService, OrganizationInviteService>();
            
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IUserOrganizationRepository, UserOrganizationRepository>();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            app.UseEndpoints(e =>
                {
                    e.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action}/{id?}",
                        defaults: new { controller = "home", Action = "index" }
                    );
                });
        }
    }
}
