using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using trycore.valor.ganado.infrastructure.DAOs.Evm;
using trycore.valor.ganado.infrastructure.Http;
using trycore.valor.ganado.infrastructure.Persistence;

namespace trycore.valor.ganado.test.Infrastructure;

/// <summary>
/// Fábrica de aplicación web para pruebas de integración.
/// Reemplaza PostgreSQL por InMemory y sustituye el cliente HTTP del calculador EVM por un mock.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Nombre único por instancia de fábrica: todos los requests dentro de la misma clase de test
    // comparten exactamente la misma base de datos InMemory.
    private readonly string _dbName = "TestDb_" + Guid.NewGuid();

    public Mock<IEvmCalculatorClient> EvmCalculatorMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Provee valores de configuración mínimos para que la app arranque sin fallar
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=localhost;Database=test_db;Username=test;Password=test",
                ["EvmCalculator:BaseUrl"] = "http://localhost:9999"
            });
        });

        // ConfigureTestServices corre DESPUÉS de todos los registros del programa,
        // lo que permite reemplazar servicios de infraestructura de forma segura.
        builder.ConfigureTestServices(services =>
        {
            ReemplazarDbContext(services);
            ReemplazarClienteEvm(services);
        });
    }

    private void ReemplazarDbContext(IServiceCollection services)
    {
        // Elimina los descriptores de DbContext existentes (registrados con Npgsql)
        var descriptorsAEliminar = services
            .Where(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                d.ServiceType == typeof(AppDbContext))
            .ToList();

        foreach (var descriptor in descriptorsAEliminar)
            services.Remove(descriptor);

        // Registra el contexto con base de datos InMemory (única por instancia de fábrica).
        // _dbName se evalúa una sola vez en el constructor, garantizando que todos los requests
        // de la misma clase de test comparten el mismo store en memoria.
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(_dbName));
    }

    private void ReemplazarClienteEvm(IServiceCollection services)
    {
        // Elimina el cliente HTTP tipado real (EvmCalculatorHttpClient)
        var descriptorsEvm = services
            .Where(d => d.ServiceType == typeof(IEvmCalculatorClient))
            .ToList();

        foreach (var descriptor in descriptorsEvm)
            services.Remove(descriptor);

        ConfigurarMockEvm();
        services.AddScoped(_ => EvmCalculatorMock.Object);
    }

    private void ConfigurarMockEvm()
    {
        EvmCalculatorMock
            .Setup(x => x.CalculateAsync(It.IsAny<IEnumerable<ActivityEvmInput>>()))
            .ReturnsAsync((IEnumerable<ActivityEvmInput> inputs) =>
            {
                var lista = inputs.ToList();

                var indicadores = new EvmIndicatorsResponse(
                    Pv: 1000m,
                    Ev: 900m,
                    Cv: -100m,
                    Sv: -100m,
                    Cpi: 0.9m,
                    Spi: 0.9m,
                    Eac: 1111.11m,
                    Vac: -111.11m,
                    CpiInterpretation: "Sobrecosto",
                    SpiInterpretation: "Retraso");

                var porActividad = lista.Select(a =>
                    new ActivityEvmResultResponse(a.ActivityId, a.Name, indicadores));

                return new ProjectEvmResponse(porActividad, indicadores);
            });
    }
}
