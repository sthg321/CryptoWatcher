using System.Text.Json;
using System.Text.Json.Serialization;
using CryptoWatcher.Integrations;

namespace CryptoWatcher.Host.Integrations;

public class CoinGeckoTokenPriceProvider : ITokenPriceProvider
{
    private static readonly Dictionary<string, string> Symbol2Id = new()
    {
        ["USDC"] = "usd-coin",
        ["wrsETH"] = "WETH",
        ["weth"] = "WETH",
        ["wstETH"] = "WETH",
        ["ETH"] = "WETH",
        ["USD₮0"] = "usd-coin",
    };

    private readonly HttpClient _client;

    public CoinGeckoTokenPriceProvider(HttpClient client)
    {
        _client = client;
    }

    public async Task<decimal> GetTokenPriceInUsdAsync(string tokenSymbol, CancellationToken ct)
    {
        var id = Symbol2Id.GetValueOrDefault(tokenSymbol) ?? tokenSymbol;
        var url = $"api/v3/simple/price?ids={id}&vs_currencies=usd";

        var response = await _client.GetAsync(url, ct);

        var json = await response.Content.ReadAsStreamAsync(ct);
        var result =
            await JsonSerializer.DeserializeAsync<Dictionary<string, TokenPriceInfo>>(json, cancellationToken: ct);

        return result![id.ToLower()].Usd;
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class TokenPriceInfo
    {
        [JsonPropertyName("usd")] public decimal Usd { get; set; }
    }
}