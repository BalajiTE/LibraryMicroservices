using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        services.AddScoped<Services.IAuthService, Services.AuthService>();
        services.AddScoped<Services.IUserService, Services.UserService>();
        services.AddScoped<Services.IRoleService, Services.RoleService>();

        return services;
    }
}
