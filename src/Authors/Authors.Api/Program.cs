using Authors.Application;
using Authors.Infrastructure;
using Shared.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLibrarySwagger("Authors API", "Library authors microservice");
builder.Services.AddAuthorsApplication();
builder.Services.AddAuthorsInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseLibrarySwaggerUi();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { service = "Authors", status = "Healthy" }));

app.Run();

public partial class Program;
