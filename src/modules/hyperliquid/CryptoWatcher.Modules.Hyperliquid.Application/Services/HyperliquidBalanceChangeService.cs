using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Modules.Hyperliquid.Specifications;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services;

public class HyperliquidBalanceChangeService : IHyperliquidBalanceChangeService
{
    private readonly IRepository<HyperliquidVaultPosition> _positionRepository;

    public HyperliquidBalanceChangeService(IRepository<HyperliquidVaultPosition> positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public async Task<List<HyperliquidDailyBalanceChange>> GetDailyBalanceChangesAsync(
        IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to,
        CancellationToken ct = default)
    {
        var positions =
            await _positionRepository.ListAsync(new HyperliquidPositionsForReportSpecification(wallets, from, to), ct);

        var result = new List<HyperliquidDailyBalanceChange>();
        foreach (var vaultPosition in positions)
        {
            HyperliquidVaultPositionSnapshot? previousSnapshot = null;
            foreach (var currentSnapshot in vaultPosition.PositionSnapshots)
            {
                previousSnapshot ??= currentSnapshot;

                var balanceChange =
                    HyperliquidDailyBalanceChange.CreateFromSnapshot(vaultPosition, previousSnapshot, currentSnapshot);

                previousSnapshot = currentSnapshot;

                result.Add(balanceChange);
            }
        }

        return result;
    }
}