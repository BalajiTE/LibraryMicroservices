using Authors.Domain.Repositories;
using Authors.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Persistence;

namespace Authors.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthorsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (Shared.Persistence.DependencyInjection.GetPersistenceProvider(configuration) == PersistenceProvider.SqlServer)
        {
            services.AddLibraryDbContext(configuration);
            services.AddScoped<IAuthorRepository, SqlAuthorRepository>();
        }
        else
        {
            services.AddSingleton<IAuthorRepository>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var dataFilePath = ResolveDataFilePath(configuration, "Authors:DataFilePath", "authors.json");
                return new JsonAuthorRepository(dataFilePath);
            });
        }

        return services;
    }

    private static string ResolveDataFilePath(IConfiguration configuration, string configKey, string defaultFileName)
    {
        var configuredPath = configuration[configKey];
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            configuredPath = Path.Combine("data", defaultFileName);
        }

        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredPath));
    }
}
