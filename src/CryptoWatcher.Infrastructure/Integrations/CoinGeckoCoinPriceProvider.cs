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

    public async Task<decimal> GetTokenPriceInUsdAsync(string platform, string address, CancellationToken ct)
    {
        return await _apiClient.GetTokenPriceInUsdAsync(
            new GetTokenPriceInUsdByPlatformAndAddressRequest(platform, address), ct);
    }
}