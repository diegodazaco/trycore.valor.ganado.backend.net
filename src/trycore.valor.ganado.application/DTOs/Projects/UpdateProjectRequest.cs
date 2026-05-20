namespace trycore.valor.ganado.application.DTOs.Projects;

public record UpdateProjectRequest(
    string Name,
    string? Description,
    DateTime CutoffDate
);
