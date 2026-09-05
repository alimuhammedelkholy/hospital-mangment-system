using HMS.Application.Abstractions;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HmsDatabase")
            ?? throw new InvalidOperationException("Connection string 'HmsDatabase' is not configured.");

        services.AddDbContext<HmsDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddHealthChecks().AddDbContextCheck<HmsDbContext>("database");
        return services;
    }
}
