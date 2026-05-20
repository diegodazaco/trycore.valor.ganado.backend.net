using Microsoft.AspNetCore.Mvc;
using trycore.valor.ganado.application.DTOs.Activities;
using trycore.valor.ganado.application.Interfaces;

namespace trycore.valor.ganado.api.Controllers;

/// <summary>
/// Gestión de actividades dentro de un proyecto.
/// Cada actividad registra los datos base del EVM: BAC, % planificado, % completado y AC.
/// Los indicadores (PV, EV, CV, SV, CPI, SPI, EAC, VAC) se obtienen a través del detalle del proyecto.
/// </summary>
[ApiController]
[Produces("application/json")]
public class ActivitiesController : ControllerBase
{
    private readonly IActivityService _activityService;

    public ActivitiesController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    /// <summary>
    /// Agrega una nueva actividad a un proyecto existente.
    /// </summary>
    /// <param name="projectId">Identificador del proyecto al que pertenece la actividad.</param>
    /// <param name="request">
    /// Datos de la actividad:
    /// - BudgetAtCompletion (BAC): presupuesto total planificado.
    /// - PlannedProgressPercentage: % de avance planificado a la fecha de corte (0–100).
    /// - ActualProgressPercentage: % de avance real completado (0–100).
    /// - ActualCost (AC): costo real incurrido hasta la fecha.
    /// </param>
    /// <returns>Actividad creada. Los indicadores EVM se calculan al consultar el proyecto.</returns>
    [HttpPost("api/projects/{projectId:guid}/activities")]
    [ProducesResponseType(typeof(ActivityResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ActivityResponse>> Create(Guid projectId, [FromBody] CreateActivityRequest request)
    {
        var created = await _activityService.CreateAsync(projectId, request);
        if (created is null)
            return NotFound(new { message = $"Proyecto con id '{projectId}' no encontrado." });

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Retorna una actividad por su identificador (sin indicadores EVM calculados).
    /// Para ver los indicadores EVM, consulte el endpoint GET /api/projects/{id}.
    /// </summary>
    /// <param name="id">Identificador único de la actividad.</param>
    [HttpGet("api/activities/{id:guid}")]
    [ProducesResponseType(typeof(ActivityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActivityResponse>> GetById(Guid id)
    {
        var activity = await _activityService.GetByIdAsync(id);
        if (activity is null)
            return NotFound(new { message = $"Actividad con id '{id}' no encontrada." });

        return Ok(activity);
    }

    /// <summary>
    /// Actualiza los datos de una actividad existente.
    /// Los indicadores EVM se recalcularán automáticamente al consultar el proyecto.
    /// </summary>
    /// <param name="id">Identificador único de la actividad a actualizar.</param>
    /// <param name="request">Nuevos datos de la actividad.</param>
    [HttpPut("api/activities/{id:guid}")]
    [ProducesResponseType(typeof(ActivityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ActivityResponse>> Update(Guid id, [FromBody] UpdateActivityRequest request)
    {
        var updated = await _activityService.UpdateAsync(id, request);
        if (updated is null)
            return NotFound(new { message = $"Actividad con id '{id}' no encontrada." });

        return Ok(updated);
    }

    /// <summary>
    /// Elimina una actividad de un proyecto.
    /// </summary>
    /// <param name="id">Identificador único de la actividad a eliminar.</param>
    [HttpDelete("api/activities/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _activityService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Actividad con id '{id}' no encontrada." });

        return NoContent();
    }
}
