using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.UniswapModule.Models;

namespace CryptoWatcher.UniswapModule.Abstractions;

public interface ITokenEnricher
{
    /// <summary>
    /// Enriches the specified token pair with additional metadata, such as symbol and price, for the given Uniswap network.
    /// </summary>
    /// <param name="network">The Uniswap network associated with the token pair.</param>
    /// <param name="tokenPair">The pair of tokens to be enriched.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation, containing a TokenInfoPair with enriched metadata.</returns>
    ValueTask<TokenInfoPair> EnrichAsync(UniswapNetwork network, TokenPair tokenPair,
        CancellationToken ct = default);
}