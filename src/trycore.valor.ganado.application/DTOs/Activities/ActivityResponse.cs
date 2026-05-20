using trycore.valor.ganado.infrastructure.DAOs.Evm;

namespace trycore.valor.ganado.application.DTOs.Activities;

public record ActivityResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    decimal BudgetAtCompletion,
    decimal PlannedProgressPercentage,
    decimal ActualProgressPercentage,
    decimal ActualCost,
    EvmIndicatorsResponse? Indicators
);
