using Books.Application;
using Books.Infrastructure;
using Shared.Api;
using Shared.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLibrarySwagger("Books API", "Library books microservice");
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddTestAuthentication();
}
else
{
    builder.Services.AddLibraryJwtAuthentication(builder.Configuration);
}
builder.Services.AddBooksApplication();
builder.Services.AddBooksInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseLibrarySwaggerUi();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { service = "Books", status = "Healthy" }));

app.Run();

public partial class Program;
