using Microsoft.Extensions.DependencyInjection;
using trycore.valor.ganado.application.Interfaces;
using trycore.valor.ganado.application.Services;

namespace trycore.valor.ganado.application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IActivityService, ActivityService>();
        return services;
    }
}
