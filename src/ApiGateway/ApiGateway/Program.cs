using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpClient("health-check");

var app = builder.Build();

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

app.MapReverseProxy();

app.Run();

public partial class Program;
