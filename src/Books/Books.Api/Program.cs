using Books.Application;
using Books.Infrastructure;
using Shared.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLibrarySwagger("Books API", "Library books microservice");
builder.Services.AddBooksApplication();
builder.Services.AddBooksInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseLibrarySwaggerUi();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { service = "Books", status = "Healthy" }));

app.Run();

public partial class Program;
