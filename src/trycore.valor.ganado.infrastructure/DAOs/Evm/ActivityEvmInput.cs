namespace trycore.valor.ganado.infrastructure.DAOs.Evm;

public record ActivityEvmInput(
    string ActivityId,
    string Name,
    decimal BudgetAtCompletion,
    decimal PlannedProgressPercentage,
    decimal ActualProgressPercentage,
    decimal ActualCost
);
