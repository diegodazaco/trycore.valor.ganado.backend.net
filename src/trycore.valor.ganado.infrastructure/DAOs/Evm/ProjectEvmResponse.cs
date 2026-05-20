namespace trycore.valor.ganado.infrastructure.DAOs.Evm;

public record ProjectEvmResponse(
    IEnumerable<ActivityEvmResultResponse> PerActivity,
    EvmIndicatorsResponse Consolidated
);
