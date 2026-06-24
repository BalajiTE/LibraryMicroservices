using Members.Domain.Repositories;
using Members.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Persistence;

namespace Members.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMembersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (Shared.Persistence.DependencyInjection.GetPersistenceProvider(configuration) == PersistenceProvider.SqlServer)
        {
            services.AddLibraryDbContext(configuration);
            services.AddScoped<IMemberRepository, SqlMemberRepository>();
        }
        else
        {
            services.AddSingleton<IMemberRepository>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var dataFilePath = ResolveDataFilePath(configuration, "Members:DataFilePath", "members.json");
                return new JsonMemberRepository(dataFilePath);
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
