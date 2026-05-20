namespace trycore.valor.ganado.application.DTOs.Projects;

public record ProjectSummaryResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CutoffDate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
