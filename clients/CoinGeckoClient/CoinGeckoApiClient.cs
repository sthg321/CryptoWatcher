using System.Net.Http.Json;
using CoinGeckoClient.CoinsList;
using CoinGeckoClient.Price;

namespace CoinGeckoClient;

public interface ICoinGeckoApiClient
{
    Task<decimal> GetTokenPriceInUsdAsync(GetTokenPriceInUsdRequest request, CancellationToken ct = default);

    Task<CoinInfo[]> GetCoinsListAsync(CancellationToken ct = default);

    Task<decimal> GetTokenPriceInUsdAsync(GetTokenPriceInUsdByPlatformAndAddressRequest request,
        CancellationToken ct);
}

public class CoinGeckoApiClient : ICoinGeckoApiClient
{
    private static readonly Dictionary<string, string> Symbol2Id = new()
    {
        ["USDC"] = "usd-coin",
        ["wrsETH"] = "WETH",
        ["weth"] = "WETH",
        ["wstETH"] = "WETH",
        ["ETH"] = "WETH",
        ["USD₮0"] = "usd-coin",
        ["wbtc"] = "btc",
    };

    private readonly HttpClient _client;

    public CoinGeckoApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<CoinInfo[]> GetCoinsListAsync(CancellationToken ct)
    {
        var result = await _client.GetFromJsonAsync<CoinInfo[]>("/api/v3/coins/list?include_platform=true", ct);

        return result!;
    }

    public async Task<decimal> GetTokenPriceInUsdAsync(GetTokenPriceInUsdRequest request, CancellationToken ct)
    {
        var id = Symbol2Id.GetValueOrDefault(request.Symbol) ?? request.Symbol;
        var url = $"api/v3/simple/price?symbols={id}&vs_currencies=usd";

        var result = await _client.GetFromJsonAsync<Dictionary<string, TokenPriceInfo>>(url, ct);

        return result![id.ToLower()].Usd;
    }

    public async Task<decimal> GetTokenPriceInUsdAsync(GetTokenPriceInUsdByPlatformAndAddressRequest request,
        CancellationToken ct)
    {
        var url =
            $"api/v3/simple/token_price/{request.Platform.ToLower()}?contract_addresses={request.TokenAddress}&vs_currencies=usd";

        var result = await _client.GetFromJsonAsync<Dictionary<string, TokenPriceInfo>>(url, ct);

        return result!.GetValueOrDefault(request.TokenAddress.ToLower())?.Usd ?? 0;
    }
}