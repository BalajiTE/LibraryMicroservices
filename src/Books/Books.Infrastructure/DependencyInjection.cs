using Books.Domain.Repositories;
using Books.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Persistence;

namespace Books.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBooksInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (Shared.Persistence.DependencyInjection.GetPersistenceProvider(configuration) == PersistenceProvider.SqlServer)
        {
            services.AddLibraryDbContext(configuration);
            services.AddScoped<IBookRepository, SqlBookRepository>();
        }
        else
        {
            services.AddSingleton<IBookRepository>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var dataFilePath = ResolveDataFilePath(configuration, "Books:DataFilePath", "books.json");
                return new JsonBookRepository(dataFilePath);
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
