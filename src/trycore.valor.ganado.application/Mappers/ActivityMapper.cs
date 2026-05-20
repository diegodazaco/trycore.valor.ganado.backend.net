using trycore.valor.ganado.application.DTOs.Activities;
using trycore.valor.ganado.infrastructure.DAOs.Evm;
using trycore.valor.ganado.domain.Entities;

namespace trycore.valor.ganado.application.Mappers;

public static class ActivityMapper
{
    public static ActivityResponse ToResponse(this Activity activity, EvmIndicatorsResponse? indicators) =>
        new(activity.Id, activity.ProjectId, activity.Name,
            activity.BudgetAtCompletion, activity.PlannedProgressPercentage,
            activity.ActualProgressPercentage, activity.ActualCost,
            indicators);

    public static ActivityEvmInput ToEvmInput(this Activity activity) =>
        new(activity.Id.ToString(), activity.Name, activity.BudgetAtCompletion,
            activity.PlannedProgressPercentage, activity.ActualProgressPercentage,
            activity.ActualCost);
}
