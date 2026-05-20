using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using trycore.valor.ganado.application.Interfaces;
using trycore.valor.ganado.infrastructure.Http;
using trycore.valor.ganado.infrastructure.Persistence;
using trycore.valor.ganado.infrastructure.Persistence.Repositories;

namespace trycore.valor.ganado.infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();

        services.AddHttpClient<IEvmCalculatorClient, EvmCalculatorHttpClient>(client =>
        {
            client.BaseAddress = new Uri(
                configuration["EvmCalculator:BaseUrl"]
                ?? throw new InvalidOperationException("EvmCalculator:BaseUrl is not configured."));
        });

        return services;
    }
}
