namespace trycore.valor.ganado.infrastructure.DAOs.Evm;

public record EvmIndicatorsResponse(
    decimal Pv,
    decimal Ev,
    decimal Cv,
    decimal Sv,
    decimal? Cpi,
    decimal? Spi,
    decimal? Eac,
    decimal? Vac,
    string CpiInterpretation,
    string SpiInterpretation
);
