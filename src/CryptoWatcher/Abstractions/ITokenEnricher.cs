using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Abstractions;

public interface ITokenEnricher
{
    /// <summary>
    /// Enriches the specified token pair with additional metadata, such as symbol and price, for the given Uniswap network.
    /// </summary>
    /// <param name="rpcAddress">Address of the token network</param>
    /// <param name="tokenPair">The pair of tokens to be enriched.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation, containing a TokenInfoPair with enriched metadata.</returns>
    ValueTask<TokenInfoPair> EnrichAsync(string rpcAddress, TokenPair tokenPair,
        CancellationToken ct = default);

    ValueTask<CryptoToken> EnrichTokenAsync(string rpcAddress, Token token,
        CancellationToken ct);

    ValueTask<CryptoToken> EnrichTokenAsync(string rpcAddress, string platform, Token token,
        CancellationToken ct);

    ValueTask<CryptoToken> EnrichTokenAsync(string rpcAddress, Token token, decimal priceInUsd,
        CancellationToken ct);
}