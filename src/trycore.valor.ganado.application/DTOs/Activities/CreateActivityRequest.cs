namespace trycore.valor.ganado.application.DTOs.Activities;

public record CreateActivityRequest(
    string Name,
    decimal BudgetAtCompletion,
    decimal PlannedProgressPercentage,
    decimal ActualProgressPercentage,
    decimal ActualCost
);
