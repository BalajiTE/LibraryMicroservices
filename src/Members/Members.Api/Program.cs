using Members.Application;
using Members.Infrastructure;
using Shared.Api;
using Shared.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLibrarySwagger("Members API", "Library members microservice");
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddTestAuthentication();
}
else
{
    builder.Services.AddLibraryJwtAuthentication(builder.Configuration);
}
builder.Services.AddMembersApplication();
builder.Services.AddMembersInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseLibrarySwaggerUi();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { service = "Members", status = "Healthy" }));

app.Run();

public partial class Program;
