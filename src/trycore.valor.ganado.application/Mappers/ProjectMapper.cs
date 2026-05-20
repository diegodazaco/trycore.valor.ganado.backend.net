using trycore.valor.ganado.application.DTOs.Activities;
using trycore.valor.ganado.infrastructure.DAOs.Evm;
using trycore.valor.ganado.application.DTOs.Projects;
using trycore.valor.ganado.domain.Entities;

namespace trycore.valor.ganado.application.Mappers;

public static class ProjectMapper
{
    public static ProjectSummaryResponse ToSummaryResponse(this Project project) =>
        new(project.Id, project.Name, project.Description, project.CutoffDate,
            project.CreatedAt, project.UpdatedAt);

    public static ProjectDetailResponse ToDetailResponse(
        this Project project,
        IEnumerable<ActivityResponse> activities,
        EvmIndicatorsResponse? consolidatedIndicators) =>
        new(project.Id, project.Name, project.Description, project.CutoffDate,
            project.CreatedAt, project.UpdatedAt, activities, consolidatedIndicators);
}
