using trycore.valor.ganado.application.DTOs.Activities;

namespace trycore.valor.ganado.application.Interfaces;

public interface IActivityService
{
    Task<ActivityResponse?> GetByIdAsync(Guid id);
    Task<ActivityResponse?> CreateAsync(Guid projectId, CreateActivityRequest request);
    Task<ActivityResponse?> UpdateAsync(Guid id, UpdateActivityRequest request);
    Task<bool> DeleteAsync(Guid id);
}
