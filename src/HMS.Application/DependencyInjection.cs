using FluentValidation;
using HMS.Application.Authentication.Abstractions;
using HMS.Application.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}
