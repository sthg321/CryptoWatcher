using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Abstractions;

public interface IAaveTokenEnricher
{
    Task<CryptoToken> EnrichTokenAsync(AaveProtocolConfiguration protocol,
        AaveLendingPosition lendingPosition,
        CancellationToken ct = default);
}