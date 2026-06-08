using Loans.Domain.Repositories;
using Loans.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Loans.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLoansInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ILoanRepository>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var dataFilePath = ResolveDataFilePath(configuration, "Loans:DataFilePath", "loans.json");
            return new JsonLoanRepository(dataFilePath);
        });
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
