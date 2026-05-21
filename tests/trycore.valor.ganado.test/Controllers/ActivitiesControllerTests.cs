using System.Net;
using System.Net.Http.Json;
using trycore.valor.ganado.application.DTOs.Activities;
using trycore.valor.ganado.application.DTOs.Projects;
using trycore.valor.ganado.test.Infrastructure;

namespace trycore.valor.ganado.test.Controllers;

public class ActivitiesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ActivitiesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_EnProyectoExistente_Retorna201ConActividadCreada()
    {
        var proyecto = await CrearProyecto("Proyecto Para Nueva Actividad");
        var request = new CreateActivityRequest(
            "Diseño de arquitectura",
            10000m,
            30m,
            25m,
            3000m);

        var response = await _client.PostAsJsonAsync(
            $"/api/projects/{proyecto.Id}/activities", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var body = await response.Content.ReadFromJsonAsync<ActivityResponse>();
        Assert.NotNull(body);
        Assert.Equal("Diseño de arquitectura", body.Name);
        Assert.Equal(proyecto.Id, body.ProjectId);
        Assert.Equal(10000m, body.BudgetAtCompletion);
        Assert.Equal(30m, body.PlannedProgressPercentage);
        Assert.Equal(25m, body.ActualProgressPercentage);
        Assert.Equal(3000m, body.ActualCost);
        Assert.NotEqual(Guid.Empty, body.Id);
    }

    [Fact]
    public async Task Create_EnProyectoNoExistente_Retorna404()
    {
        var request = new CreateActivityRequest("Actividad Huérfana", 5000m, 50m, 50m, 2500m);

        var response = await _client.PostAsJsonAsync(
            $"/api/projects/{Guid.NewGuid()}/activities", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ActividadExistente_Retorna200ConDatosCorrectos()
    {
        var actividad = await CrearActividadConProyecto("Actividad A Consultar");

        var response = await _client.GetAsync($"/api/activities/{actividad.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ActivityResponse>();
        Assert.NotNull(body);
        Assert.Equal(actividad.Id, body.Id);
        Assert.Equal("Actividad A Consultar", body.Name);
        Assert.Equal(actividad.ProjectId, body.ProjectId);
    }

    [Fact]
    public async Task GetById_ActividadNoExistente_Retorna404()
    {
        var response = await _client.GetAsync($"/api/activities/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task Update_ActividadExistente_Retorna200ConDatosActualizados()
    {
        var actividad = await CrearActividadConProyecto("Actividad Original");
        var updateRequest = new UpdateActivityRequest(
            "Actividad Actualizada",
            15000m,
            70m,
            65m,
            10000m);

        var response = await _client.PutAsJsonAsync($"/api/activities/{actividad.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ActivityResponse>();
        Assert.NotNull(body);
        Assert.Equal("Actividad Actualizada", body.Name);
        Assert.Equal(15000m, body.BudgetAtCompletion);
        Assert.Equal(70m, body.PlannedProgressPercentage);
        Assert.Equal(65m, body.ActualProgressPercentage);
        Assert.Equal(10000m, body.ActualCost);
    }

    [Fact]
    public async Task Update_ActividadNoExistente_Retorna404()
    {
        var updateRequest = new UpdateActivityRequest("Nombre", 1000m, 50m, 50m, 500m);

        var response = await _client.PutAsJsonAsync($"/api/activities/{Guid.NewGuid()}", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ActividadExistente_Retorna204SinContenido()
    {
        var actividad = await CrearActividadConProyecto("Actividad A Eliminar");

        var response = await _client.DeleteAsync($"/api/activities/{actividad.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ActividadNoExistente_Retorna404()
    {
        var response = await _client.DeleteAsync($"/api/activities/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<ProjectSummaryResponse> CrearProyecto(string nombre)
    {
        var request = new CreateProjectRequest(nombre, null, DateTime.UtcNow.AddMonths(3));
        var response = await _client.PostAsJsonAsync("/api/projects", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ProjectSummaryResponse>())!;
    }

    private async Task<ActivityResponse> CrearActividadConProyecto(string nombreActividad)
    {
        var proyecto = await CrearProyecto($"Proyecto base: {nombreActividad}");
        var request = new CreateActivityRequest(nombreActividad, 8000m, 40m, 35m, 3200m);
        var response = await _client.PostAsJsonAsync(
            $"/api/projects/{proyecto.Id}/activities", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ActivityResponse>())!;
    }
}
