using System.Net.Http.Json;
using System.Text.Json;
using trycore.valor.ganado.infrastructure.DAOs.Evm;

namespace trycore.valor.ganado.infrastructure.Http;

public class EvmCalculatorHttpClient : IEvmCalculatorClient
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };

    public EvmCalculatorHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProjectEvmResponse> CalculateAsync(IEnumerable<ActivityEvmInput> activities)
    {
        var requestBody = new { activities };
        var response = await _httpClient.PostAsJsonAsync("/calculate", requestBody, JsonOptions);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ProjectEvmResponse>(JsonOptions)
            ?? throw new InvalidOperationException("EVM calculator service returned an empty response.");
    }
}
