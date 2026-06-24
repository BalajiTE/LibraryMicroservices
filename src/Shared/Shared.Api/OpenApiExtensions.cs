using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Shared.Api;

public static class OpenApiExtensions
{
    private static readonly Dictionary<int, string> PortToOpenApiPrefix = new()
    {
        [5101] = "authors",
        [5102] = "books",
        [5103] = "loans",
        [5104] = "members"
    };

    public static IServiceCollection AddLibrarySwagger(
        this IServiceCollection services,
        string apiTitle,
        string? description = null)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = apiTitle,
                Version = "v1",
                Description = description
            });
        });

        return services;
    }

    public static WebApplication UseLibrarySwaggerUi(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", app.Environment.ApplicationName);
            options.RoutePrefix = "swagger";
        });

        return app;
    }

    public static WebApplication UseGatewaySwaggerUi(
        this WebApplication app,
        params (string Name, string SwaggerUrl)[] downstreamApis)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "swagger";

            foreach (var (name, swaggerUrl) in downstreamApis)
            {
                options.SwaggerEndpoint(swaggerUrl, name);
            }
        });

        return app;
    }

    public static WebApplication MapGatewayOpenApiProxies(
        this WebApplication app,
        IConfiguration configuration,
        string httpClientName = "openapi-proxy")
    {
        foreach (var serviceUrl in configuration.GetSection("ServiceHealthChecks").Get<string[]>() ?? [])
        {
            if (!Uri.TryCreate(serviceUrl, UriKind.Absolute, out var uri))
            {
                continue;
            }

            if (!PortToOpenApiPrefix.TryGetValue(uri.Port, out var prefix))
            {
                continue;
            }

            var upstream = serviceUrl.TrimEnd('/');
            app.MapGet($"/openapi/{prefix}/v1/swagger.json", async (IHttpClientFactory httpClientFactory) =>
            {
                var client = httpClientFactory.CreateClient(httpClientName);

                try
                {
                    using var response = await client.GetAsync($"{upstream}/swagger/v1/swagger.json");
                    if (!response.IsSuccessStatusCode)
                    {
                        return Results.Json(
                            new
                            {
                                title = $"{prefix} OpenAPI unavailable",
                                status = (int)response.StatusCode,
                                detail = $"Upstream at {upstream} returned {(int)response.StatusCode}. Ensure the service is running with ASPNETCORE_ENVIRONMENT=Development."
                            },
                            statusCode: (int)response.StatusCode);
                    }

                    return Results.Content(
                        await response.Content.ReadAsStringAsync(),
                        "application/json");
                }
                catch (HttpRequestException ex)
                {
                    return Results.Json(
                        new
                        {
                            title = $"{prefix} service unreachable",
                            status = StatusCodes.Status503ServiceUnavailable,
                            detail = $"Could not connect to {upstream}. Start the service and verify {upstream}/health responds.",
                            error = ex.Message
                        },
                        statusCode: StatusCodes.Status503ServiceUnavailable);
                }
            });
        }

        return app;
    }

    public static (string Name, string SwaggerUrl)[] GetGatewaySwaggerEndpoints(IConfiguration configuration)
    {
        return (configuration.GetSection("ServiceHealthChecks").Get<string[]>() ?? [])
            .Select(serviceUrl =>
            {
                if (!Uri.TryCreate(serviceUrl, UriKind.Absolute, out var uri)
                    || !PortToOpenApiPrefix.TryGetValue(uri.Port, out var prefix))
                {
                    return (serviceUrl, $"/openapi/unknown/v1/swagger.json");
                }

                var name = uri.Port switch
                {
                    5101 => "Authors API",
                    5102 => "Books API",
                    5103 => "Loans API",
                    5104 => "Members API",
                    _ => serviceUrl
                };

                return (name, $"/openapi/{prefix}/v1/swagger.json");
            })
            .ToArray();
    }
}
