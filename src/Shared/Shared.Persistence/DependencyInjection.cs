using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Persistence;

public static class DependencyInjection
{
    public const string ConnectionStringName = "LibraryDatabase";
    public const string ProviderConfigKey = "Persistence:Provider";

    public static IServiceCollection AddLibraryDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is required for SQL Server persistence.");

        services.AddDbContext<LibraryDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    public static PersistenceProvider GetPersistenceProvider(IConfiguration configuration) =>
        configuration.GetValue<PersistenceProvider?>(ProviderConfigKey) ?? PersistenceProvider.Json;
}
