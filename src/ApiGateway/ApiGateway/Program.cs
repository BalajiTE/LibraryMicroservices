using System.Net;
using Shared.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpClient("health-check");
builder.Services.AddHttpClient("openapi-proxy", client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

app.UseCors("Frontend");

app.MapGet("/health", () => Results.Ok(new { service = "ApiGateway", status = "Healthy" }));

app.MapGet("/health/services", async (IHttpClientFactory httpClientFactory, IConfiguration configuration) =>
{
    var client = httpClientFactory.CreateClient("health-check");
    var services = configuration.GetSection("ServiceHealthChecks").Get<string[]>() ?? [];

    var results = new List<object>();
    foreach (var serviceUrl in services)
    {
        var name = new Uri(serviceUrl).Port switch
        {
            5101 => "Authors",
            5102 => "Books",
            5103 => "Loans",
            5104 => "Members",
            5105 => "Auth",
            _ => serviceUrl
        };

        try
        {
            var response = await client.GetAsync($"{serviceUrl.TrimEnd('/')}/health");
            results.Add(new
            {
                service = name,
                url = serviceUrl,
                status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                statusCode = (int)response.StatusCode
            });
        }
        catch (Exception ex)
        {
            results.Add(new
            {
                service = name,
                url = serviceUrl,
                status = "Unreachable",
                error = ex.Message
            });
        }
    }

    var allHealthy = results.All(result =>
        result.GetType().GetProperty("status")?.GetValue(result)?.ToString() == "Healthy");

    return allHealthy
        ? Results.Ok(new { gateway = "Healthy", services = results })
        : Results.Json(new { gateway = "Degraded", services = results }, statusCode: (int)HttpStatusCode.ServiceUnavailable);
});

var swaggerServices = OpenApiExtensions.GetGatewaySwaggerEndpoints(app.Configuration);

app.MapGatewayOpenApiProxies(app.Configuration);
app.UseGatewaySwaggerUi(swaggerServices);
app.MapReverseProxy();

app.Run();

public partial class Program;
