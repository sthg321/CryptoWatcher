using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.UniswapModule.Models;

namespace CryptoWatcher.UniswapModule.Abstractions;

public interface ITokenEnricher
{
    ValueTask<TokenInfoPair> EnrichAsync(UniswapNetwork network, TokenPair tokenPair,
        CancellationToken ct = default);
}