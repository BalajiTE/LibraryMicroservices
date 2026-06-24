using Members.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Members.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMembersApplication(this IServiceCollection services)
    {
        services.AddScoped<IMemberService, MemberService>();
        return services;
    }
}
