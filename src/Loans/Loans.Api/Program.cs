using Loans.Application;
using Loans.Infrastructure;
using Shared.Api;
using Shared.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLibrarySwagger("Loans API", "Library loans microservice");
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddTestAuthentication();
}
else
{
    builder.Services.AddLibraryJwtAuthentication(builder.Configuration);
}
builder.Services.AddLoansApplication();
builder.Services.AddLoansInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseLibrarySwaggerUi();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { service = "Loans", status = "Healthy" }));

app.Run();

public partial class Program;
