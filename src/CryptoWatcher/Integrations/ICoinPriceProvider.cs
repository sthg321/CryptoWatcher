namespace CryptoWatcher.Integrations;

/// <summary>
/// Provides the current price of tokens in USD.
/// </summary>
public interface ICoinPriceProvider
{
    Task<decimal> GetTokenPriceInUsdAsync(string platform, string address, CancellationToken ct);
}