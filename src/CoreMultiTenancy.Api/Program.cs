var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
