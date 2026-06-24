using Loans.Application;
using Loans.Infrastructure;
using Shared.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLibrarySwagger("Loans API", "Library loans microservice");
builder.Services.AddLoansApplication();
builder.Services.AddLoansInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseLibrarySwaggerUi();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { service = "Loans", status = "Healthy" }));

app.Run();

public partial class Program;
