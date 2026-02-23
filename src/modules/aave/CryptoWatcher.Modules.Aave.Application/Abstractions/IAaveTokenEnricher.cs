using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Abstractions;

public interface IAaveTokenEnricher
{
    Task<CryptoToken> EnrichTokenAsync(AaveChainConfiguration chain,
        AaveLendingPosition lendingPosition,
        CancellationToken ct = default);
}