using Books.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Books.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBooksApplication(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        return services;
    }
}
