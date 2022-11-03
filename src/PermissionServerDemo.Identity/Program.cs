using Microsoft.AspNetCore.HttpOverrides;
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
else
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRazorPagesNotFoundFilter("/error/notfound");

app.UseRouting();
app.UseGrpcWeb();
app.UseCors();
app.UseIdentityServer();
app.UseAuthorization();

app.UseEndpoints(e =>
{
    e.MapRazorPages()
        .RequireAuthorization();
    e.MapControllers();
    e.MapGrpcAuthorizationServices();
});

app.Run();
