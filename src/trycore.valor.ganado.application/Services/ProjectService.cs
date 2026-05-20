using trycore.valor.ganado.application.DTOs.Projects;
using trycore.valor.ganado.application.Interfaces;
using trycore.valor.ganado.application.Mappers;
using trycore.valor.ganado.domain.Entities;
using trycore.valor.ganado.infrastructure.Http;

namespace trycore.valor.ganado.application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IEvmCalculatorClient _evmCalculatorClient;

    public ProjectService(IProjectRepository projectRepository, IEvmCalculatorClient evmCalculatorClient)
    {
        _projectRepository = projectRepository;
        _evmCalculatorClient = evmCalculatorClient;
    }

    public async Task<IEnumerable<ProjectSummaryResponse>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        return projects.Select(p => p.ToSummaryResponse());
    }

    public async Task<ProjectDetailResponse?> GetByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdWithActivitiesAsync(id);
        if (project is null)
            return null;

        if (!project.Activities.Any())
            return project.ToDetailResponse([], null);

        var evmInputs = project.Activities.Select(a => a.ToEvmInput());
        var evmResult = await _evmCalculatorClient.CalculateAsync(evmInputs);

        var evmByActivityId = evmResult.PerActivity.ToDictionary(r => r.ActivityId);
        var activities = project.Activities.Select(a =>
            a.ToResponse(evmByActivityId.GetValueOrDefault(a.Id.ToString())?.Indicators));

        return project.ToDetailResponse(activities, evmResult.Consolidated);
    }

    public async Task<ProjectSummaryResponse> CreateAsync(CreateProjectRequest request)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CutoffDate = request.CutoffDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _projectRepository.AddAsync(project);
        return project.ToSummaryResponse();
    }

    public async Task<ProjectSummaryResponse?> UpdateAsync(Guid id, UpdateProjectRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project is null)
            return null;

        project.Name = request.Name;
        project.Description = request.Description;
        project.CutoffDate = request.CutoffDate;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);
        return project.ToSummaryResponse();
    }

    public async Task<bool> DeleteAsync(Guid id) =>
        await _projectRepository.DeleteAsync(id);
}
