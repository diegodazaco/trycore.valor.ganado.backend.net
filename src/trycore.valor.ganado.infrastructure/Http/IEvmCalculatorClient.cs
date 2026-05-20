using trycore.valor.ganado.infrastructure.DAOs.Evm;

namespace trycore.valor.ganado.infrastructure.Http;

public interface IEvmCalculatorClient
{
    Task<ProjectEvmResponse> CalculateAsync(IEnumerable<ActivityEvmInput> activities);
}
