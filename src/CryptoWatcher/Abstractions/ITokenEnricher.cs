using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Abstractions;

public interface ITokenEnricher
{
    ValueTask<CryptoToken> EnrichAsync(string networkName, Uri rpcAddress, Token token,
        CancellationToken ct = default);
    
    ValueTask<TokenInfoPair> EnrichAsync(string networkName, Uri rpcAddress, TokenPair tokenPair,
        CancellationToken ct = default);

    ValueTask<CryptoToken> EnrichTokenAsync(Uri rpcAddress, Token token, decimal priceInUsd,
        CancellationToken ct);
}