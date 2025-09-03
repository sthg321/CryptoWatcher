using CoinGeckoClient;
using CoinGeckoClient.Price;
using CryptoWatcher.Integrations;

namespace CryptoWatcher.Infrastructure.Integrations;

public class CoinGeckoCoinPriceProvider : ICoinPriceProvider
{
    private readonly ICoinGeckoApiClient _apiClient;

    public CoinGeckoCoinPriceProvider(ICoinGeckoApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<Dictionary<string, string>> GetSymbolToIdAsync(CancellationToken ct)
    {
        var coinsInfo = await _apiClient.GetCoinsListAsync(ct);

        return coinsInfo
            .GroupBy(info => info.Symbol)
            .ToDictionary(info => info.Key, info => info.Select(coinInfo => coinInfo).First().Id);
    }

    public async Task<decimal> GetTokenPriceInUsdAsync(string tokenId, CancellationToken ct)
    {
        return await _apiClient.GetTokenPriceInUsdAsync(new GetTokenPriceInUsdRequest(tokenId.ToLower()), ct);
    }
}