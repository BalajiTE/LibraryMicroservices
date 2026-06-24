using Auth.Application.Services;
using Auth.Domain.Repositories;
using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Persistence;

namespace Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        if (Shared.Persistence.DependencyInjection.GetPersistenceProvider(configuration) == PersistenceProvider.SqlServer)
        {
            services.AddLibraryDbContext(configuration);
            services.AddScoped<IUserRepository, SqlUserRepository>();
            services.AddScoped<IRoleRepository, SqlRoleRepository>();
            services.AddScoped<IUserRoleRepository, SqlUserRoleRepository>();
        }
        else
        {
            services.AddSingleton<IRoleRepository>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var path = ResolveDataFilePath(config, "Auth:RolesDataFilePath", "roles.json");
                return new JsonRoleRepository(path);
            });

            services.AddSingleton<IUserRepository>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var path = ResolveDataFilePath(config, "Auth:UsersDataFilePath", "users.json");
                return new JsonUserRepository(path);
            });

            services.AddSingleton<IUserRoleRepository>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var path = ResolveDataFilePath(config, "Auth:UserRolesDataFilePath", "userRoles.json");
                var roleRepository = sp.GetRequiredService<IRoleRepository>();
                return new JsonUserRoleRepository(path, roleRepository);
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
