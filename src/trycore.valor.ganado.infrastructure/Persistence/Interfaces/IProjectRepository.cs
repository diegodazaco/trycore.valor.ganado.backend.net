using trycore.valor.ganado.domain.Entities;

namespace trycore.valor.ganado.application.Interfaces;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project?> GetByIdWithActivitiesAsync(Guid id);
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task<bool> DeleteAsync(Guid id);
}
