using Microsoft.AspNetCore.Mvc;
using trycore.valor.ganado.application.DTOs.Projects;
using trycore.valor.ganado.application.Interfaces;

namespace trycore.valor.ganado.api.Controllers;

/// <summary>
/// Gestión de proyectos con cálculo de indicadores EVM consolidados.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Retorna la lista de todos los proyectos sin indicadores EVM.
    /// </summary>
    /// <returns>Lista de proyectos con información básica.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectSummaryResponse>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }

    /// <summary>
    /// Retorna un proyecto con todas sus actividades e indicadores EVM calculados.
    /// Los indicadores se calculan en tiempo real mediante el microservicio FastAPI.
    /// </summary>
    /// <param name="id">Identificador único del proyecto.</param>
    /// <returns>
    /// Detalle del proyecto con CPI, SPI, EV, PV, AC por actividad y consolidados.
    /// Un CPI mayor a 1 indica bajo presupuesto; menor a 1 indica sobrecosto.
    /// Un SPI mayor a 1 indica adelanto; menor a 1 indica retraso.
    /// </returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDetailResponse>> GetById(Guid id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project is null)
            return NotFound(new { message = $"Proyecto con id '{id}' no encontrado." });

        return Ok(project);
    }

    /// <summary>
    /// Crea un nuevo proyecto.
    /// </summary>
    /// <param name="request">Datos del proyecto a crear.</param>
    /// <returns>Proyecto creado con su identificador único.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectSummaryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectSummaryResponse>> Create([FromBody] CreateProjectRequest request)
    {
        var created = await _projectService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Actualiza los datos de un proyecto existente.
    /// </summary>
    /// <param name="id">Identificador único del proyecto a actualizar.</param>
    /// <param name="request">Nuevos datos del proyecto.</param>
    /// <returns>Proyecto actualizado.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProjectSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectSummaryResponse>> Update(Guid id, [FromBody] UpdateProjectRequest request)
    {
        var updated = await _projectService.UpdateAsync(id, request);
        if (updated is null)
            return NotFound(new { message = $"Proyecto con id '{id}' no encontrado." });

        return Ok(updated);
    }

    /// <summary>
    /// Elimina un proyecto y todas sus actividades asociadas (eliminación en cascada).
    /// </summary>
    /// <param name="id">Identificador único del proyecto a eliminar.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _projectService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Proyecto con id '{id}' no encontrado." });

        return NoContent();
    }
}
