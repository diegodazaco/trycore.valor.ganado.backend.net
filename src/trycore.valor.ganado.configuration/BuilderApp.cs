using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using trycore.valor.ganado.application;
using trycore.valor.ganado.infrastructure;
using trycore.valor.ganado.infrastructure.Persistence;

namespace trycore.valor.ganado.configuration
{
    public static class BuilderApp
    {
        public static async Task RunApiAsync(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Trycore — API de Valor Ganado (EVM)",
                    Version = "v1",
                    Description = "API REST para gestión de proyectos con cálculo automático " +
                                  "de indicadores Earned Value Management (EVM). " +
                                  "Los cálculos son procesados de forma asíncrona por el " +
                                  "microservicio FastAPI de Python.",
                });

                var xmlFile = $"{System.Reflection.Assembly.GetEntryAssembly()!.GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (System.IO.File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            // Ejecutar migraciones pendientes automáticamente al iniciar
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("Migrations");
                try
                {
                    await db.Database.MigrateAsync();
                    logger.LogInformation("Migraciones aplicadas correctamente.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al aplicar migraciones de la base de datos.");
                    throw;
                }
            }

            // CORS debe ir primero para cubrir preflight y cualquier redirección
            app.UseCors();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Trycore EVM API v1");
                options.RoutePrefix = "swagger-ui";
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapGet("/", () => Results.Redirect("/swagger-ui")).ExcludeFromDescription();
            app.MapControllers();
            await app.RunAsync();
        }
    }
}
