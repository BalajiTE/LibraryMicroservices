using Authors.Application;
using Authors.Infrastructure;
using Shared.Api;
using Shared.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLibrarySwagger("Authors API", "Library authors microservice");
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddTestAuthentication();
}
else
{
    builder.Services.AddLibraryJwtAuthentication(builder.Configuration);
}
builder.Services.AddAuthorsApplication();
builder.Services.AddAuthorsInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseLibrarySwaggerUi();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { service = "Authors", status = "Healthy" }));

app.Run();

public partial class Program;
