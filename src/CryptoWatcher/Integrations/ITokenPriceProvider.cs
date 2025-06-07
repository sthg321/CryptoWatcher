namespace CryptoWatcher.Abstractions.Integrations;

/// <summary>
/// Provides the current price of tokens in USD.
/// </summary>
public interface ITokenPriceProvider
{
    /// <summary>
    /// Gets the current USD price of the token by its address.
    /// </summary>
    /// <param name="tokenSymbol">The ERC-20 token contract address.</param>
    /// <param name="ct">Cancellation token for the async operation.</param>
    /// <returns>Token price in USD as a decimal.</returns>
    Task<decimal> GetTokenPriceInUsdAsync(string tokenSymbol, CancellationToken ct);
}