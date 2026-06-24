using Auth.Application;
using Auth.Application.DTOs;
using Auth.Application.Services;
using Auth.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Api;
using Shared.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLibrarySwagger("Auth API", "Authentication, users, and roles");
builder.Services.AddLibraryJwtAuthentication(builder.Configuration);
builder.Services.AddAuthApplication();
builder.Services.AddAuthInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseLibrarySwaggerUi();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { service = "Auth", status = "Healthy" }));

app.Run();

public partial class Program;
