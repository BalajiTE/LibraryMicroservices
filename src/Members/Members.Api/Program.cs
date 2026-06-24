using Members.Application;
using Members.Infrastructure;
using Shared.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLibrarySwagger("Members API", "Library members microservice");
builder.Services.AddMembersApplication();
builder.Services.AddMembersInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseLibrarySwaggerUi();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { service = "Members", status = "Healthy" }));

app.Run();

public partial class Program;
