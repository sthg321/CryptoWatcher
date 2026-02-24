using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Abstractions;

public interface IAavePositionRepository
{
    Task<IReadOnlyList<AavePosition>> GetActiveForWalletAsync(
        string network,
        EvmAddress wallet,
        DateOnly day,
        CancellationToken ct);

    void Add(AavePosition position);
    
    void Update(AavePosition position);
    
    Task SaveAsync(CancellationToken ct);
}