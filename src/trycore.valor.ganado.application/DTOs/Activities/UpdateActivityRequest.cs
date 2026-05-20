namespace trycore.valor.ganado.application.DTOs.Activities;

public record UpdateActivityRequest(
    string Name,
    decimal BudgetAtCompletion,
    decimal PlannedProgressPercentage,
    decimal ActualProgressPercentage,
    decimal ActualCost
);
