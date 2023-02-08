using PermissionServer;
using PermissionServerDemo.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();

var app = builder.ConfigureServices();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRazorPagesNotFoundFilter("/error/notfound");

app.UseRouting();
app.UseCors();
app.UseIdentityServer();
app.UseAuthorization();
app.UsePermissionServer();

app.UseEndpoints(e =>
{
    e.MapRazorPages()
        .RequireAuthorization();
    e.MapControllers();
});

app.Run();
