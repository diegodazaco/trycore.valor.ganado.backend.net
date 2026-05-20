using trycore.valor.ganado.domain.Entities;

namespace trycore.valor.ganado.application.Interfaces;

public interface IActivityRepository
{
    Task<IEnumerable<Activity>> GetByProjectIdAsync(Guid projectId);
    Task<Activity?> GetByIdAsync(Guid id);
    Task AddAsync(Activity activity);
    Task UpdateAsync(Activity activity);
    Task<bool> DeleteAsync(Guid id);
}
