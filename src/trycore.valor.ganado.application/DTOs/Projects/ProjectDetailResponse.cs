using trycore.valor.ganado.application.DTOs.Activities;
using trycore.valor.ganado.infrastructure.DAOs.Evm;

namespace trycore.valor.ganado.application.DTOs.Projects;

public record ProjectDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CutoffDate,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IEnumerable<ActivityResponse> Activities,
    EvmIndicatorsResponse? ConsolidatedIndicators
);
