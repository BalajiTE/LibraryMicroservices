using Loans.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Loans.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddLoansApplication(this IServiceCollection services)
    {
        services.AddScoped<ILoanService, LoanService>();
        return services;
    }
}
