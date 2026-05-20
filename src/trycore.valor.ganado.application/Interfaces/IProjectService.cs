using trycore.valor.ganado.application.DTOs.Projects;

namespace trycore.valor.ganado.application.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectSummaryResponse>> GetAllAsync();
    Task<ProjectDetailResponse?> GetByIdAsync(Guid id);
    Task<ProjectSummaryResponse> CreateAsync(CreateProjectRequest request);
    Task<ProjectSummaryResponse?> UpdateAsync(Guid id, UpdateProjectRequest request);
    Task<bool> DeleteAsync(Guid id);
}
