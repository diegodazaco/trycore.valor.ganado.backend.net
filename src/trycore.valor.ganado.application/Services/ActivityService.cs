using trycore.valor.ganado.application.DTOs.Activities;
using trycore.valor.ganado.application.Interfaces;
using trycore.valor.ganado.application.Mappers;
using trycore.valor.ganado.domain.Entities;

namespace trycore.valor.ganado.application.Services;

public class ActivityService : IActivityService
{
    private readonly IActivityRepository _activityRepository;
    private readonly IProjectRepository _projectRepository;

    public ActivityService(IActivityRepository activityRepository, IProjectRepository projectRepository)
    {
        _activityRepository = activityRepository;
        _projectRepository = projectRepository;
    }

    public async Task<ActivityResponse?> GetByIdAsync(Guid id)
    {
        var activity = await _activityRepository.GetByIdAsync(id);
        return activity?.ToResponse(null);
    }

    public async Task<ActivityResponse?> CreateAsync(Guid projectId, CreateActivityRequest request)
    {
        var projectExists = await _projectRepository.GetByIdAsync(projectId);
        if (projectExists is null)
            return null;

        var activity = new Activity
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Name = request.Name,
            BudgetAtCompletion = request.BudgetAtCompletion,
            PlannedProgressPercentage = request.PlannedProgressPercentage,
            ActualProgressPercentage = request.ActualProgressPercentage,
            ActualCost = request.ActualCost,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _activityRepository.AddAsync(activity);
        return activity.ToResponse(null);
    }

    public async Task<ActivityResponse?> UpdateAsync(Guid id, UpdateActivityRequest request)
    {
        var activity = await _activityRepository.GetByIdAsync(id);
        if (activity is null)
            return null;

        activity.Name = request.Name;
        activity.BudgetAtCompletion = request.BudgetAtCompletion;
        activity.PlannedProgressPercentage = request.PlannedProgressPercentage;
        activity.ActualProgressPercentage = request.ActualProgressPercentage;
        activity.ActualCost = request.ActualCost;
        activity.UpdatedAt = DateTime.UtcNow;

        await _activityRepository.UpdateAsync(activity);
        return activity.ToResponse(null);
    }

    public async Task<bool> DeleteAsync(Guid id) =>
        await _activityRepository.DeleteAsync(id);
}
