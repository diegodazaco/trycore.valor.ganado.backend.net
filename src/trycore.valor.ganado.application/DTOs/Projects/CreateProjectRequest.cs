namespace trycore.valor.ganado.application.DTOs.Projects;

public record CreateProjectRequest(
    string Name,
    string? Description,
    DateTime CutoffDate
);
