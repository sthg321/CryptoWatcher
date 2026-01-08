using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Abstractions;

public interface ITokenEnricher
{
    ValueTask<TokenInfoPair> EnrichAsync(string networkName, Uri rpcAddress, TokenPair tokenPair,
        CancellationToken ct = default);

    ValueTask<CryptoToken> EnrichTokenAsync(Uri rpcAddress, Token token, decimal priceInUsd,
        CancellationToken ct);
}