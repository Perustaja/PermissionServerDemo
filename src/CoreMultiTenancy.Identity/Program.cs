using CoreMultiTenancy.Identity;

var builder = WebApplication.CreateBuilder(args);

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
