namespace trycore.valor.ganado.infrastructure.DAOs.Evm;

public record ActivityEvmResultResponse(
    string ActivityId,
    string Name,
    EvmIndicatorsResponse Indicators
);
