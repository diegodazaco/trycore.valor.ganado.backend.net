using System.Net;
using System.Net.Http.Json;
using trycore.valor.ganado.application.DTOs.Activities;
using trycore.valor.ganado.application.DTOs.Projects;
using trycore.valor.ganado.test.Infrastructure;

namespace trycore.valor.ganado.test.Controllers;

/// <summary>
/// Pruebas de integración para ProjectsController.
/// Cubre todos los endpoints: GET /api/projects, GET /api/projects/{id},
/// POST /api/projects, PUT /api/projects/{id}, DELETE /api/projects/{id}.
/// </summary>
public class ProjectsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProjectsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ─── POST /api/projects ───────────────────────────────────────────────────

    [Fact]
    public async Task Create_ConDatosValidos_Retorna201ConProyectoCreado()
    {
        var request = new CreateProjectRequest(
            "Proyecto Alpha",
            "Descripción de prueba",
            DateTime.UtcNow.AddMonths(3));

        var response = await _client.PostAsJsonAsync("/api/projects", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var body = await response.Content.ReadFromJsonAsync<ProjectSummaryResponse>();
        Assert.NotNull(body);
        Assert.Equal("Proyecto Alpha", body.Name);
        Assert.Equal("Descripción de prueba", body.Description);
        Assert.NotEqual(Guid.Empty, body.Id);
    }

    // ─── GET /api/projects ───────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_Retorna200ConListaQueContieneProyectosCreados()
    {
        await CrearProyecto("Proyecto Lista Beta");

        var response = await _client.GetAsync("/api/projects");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<ProjectSummaryResponse>>();
        Assert.NotNull(body);
        Assert.Contains(body, p => p.Name == "Proyecto Lista Beta");
    }

    // ─── GET /api/projects/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task GetById_ProyectoSinActividades_Retorna200ConIndicadoresNulos()
    {
        var created = await CrearProyecto("Proyecto Sin Actividades");

        var response = await _client.GetAsync($"/api/projects/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ProjectDetailResponse>();
        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Id);
        Assert.Equal("Proyecto Sin Actividades", body.Name);
        Assert.Empty(body.Activities);
        Assert.Null(body.ConsolidatedIndicators);
    }

    [Fact]
    public async Task GetById_ProyectoConActividades_Retorna200ConIndicadoresEvmCalculados()
    {
        var proyecto = await CrearProyecto("Proyecto Con Actividades EVM");
        await _client.PostAsJsonAsync(
            $"/api/projects/{proyecto.Id}/activities",
            new CreateActivityRequest("Fase 1 Diseño", 10000m, 40m, 35m, 4000m));

        var response = await _client.GetAsync($"/api/projects/{proyecto.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ProjectDetailResponse>();
        Assert.NotNull(body);
        Assert.NotEmpty(body.Activities);
        Assert.NotNull(body.ConsolidatedIndicators);
        Assert.NotNull(body.ConsolidatedIndicators.Cpi);
        Assert.NotNull(body.ConsolidatedIndicators.Spi);
    }

    [Fact]
    public async Task GetById_ProyectoNoExistente_Retorna404()
    {
        var response = await _client.GetAsync($"/api/projects/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ─── PUT /api/projects/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task Update_ProyectoExistente_Retorna200ConDatosActualizados()
    {
        var created = await CrearProyecto("Proyecto Delta Original");
        var updateRequest = new UpdateProjectRequest(
            "Proyecto Delta Actualizado",
            "Descripción actualizada",
            DateTime.UtcNow.AddMonths(6));

        var response = await _client.PutAsJsonAsync($"/api/projects/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ProjectSummaryResponse>();
        Assert.NotNull(body);
        Assert.Equal("Proyecto Delta Actualizado", body.Name);
        Assert.Equal("Descripción actualizada", body.Description);
        Assert.Equal(created.Id, body.Id);
    }

    [Fact]
    public async Task Update_ProyectoNoExistente_Retorna404()
    {
        var updateRequest = new UpdateProjectRequest("Cualquier Nombre", null, DateTime.UtcNow);

        var response = await _client.PutAsJsonAsync($"/api/projects/{Guid.NewGuid()}", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ─── DELETE /api/projects/{id} ────────────────────────────────────────────

    [Fact]
    public async Task Delete_ProyectoExistente_Retorna204SinContenido()
    {
        var created = await CrearProyecto("Proyecto A Eliminar");

        var response = await _client.DeleteAsync($"/api/projects/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ProyectoNoExistente_Retorna404()
    {
        var response = await _client.DeleteAsync($"/api/projects/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private async Task<ProjectSummaryResponse> CrearProyecto(string nombre)
    {
        var request = new CreateProjectRequest(nombre, null, DateTime.UtcNow.AddMonths(3));
        var response = await _client.PostAsJsonAsync("/api/projects", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ProjectSummaryResponse>())!;
    }
}
