namespace CryptoWatcher.Integrations;

/// <summary>
/// Provides the current price of tokens in USD.
/// </summary>
public interface ICoinPriceProvider
{
    Task<Dictionary<string, string>> GetSymbolToIdAsync(CancellationToken ct);
    
    /// <summary>
    /// Gets the current USD price of the token by its address.
    /// </summary>
    /// <param name="tokenId">The ERC-20 token contract address.</param>
    /// <param name="ct">Cancellation token for the async operation.</param>
    /// <returns>Token price in USD as a decimal.</returns>
    Task<decimal> GetTokenPriceInUsdAsync(string tokenId, CancellationToken ct);
    
    Task<decimal> GetTokenPriceInUsdAsync(string platform, string address, CancellationToken ct);
}