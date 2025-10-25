using CryptoWatcher.Abstractions;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Modules.Hyperliquid.Specifications;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services;

public class HyperliquidBalanceChangeService :
    BaseDailyBalanceChangeSynchronizer<HyperliquidDailyBalanceChange>,
    IDailyBalanceChangeSynchronizer
{
    private readonly IRepository<HyperliquidVaultPosition> _positionRepository;

    public HyperliquidBalanceChangeService(IRepository<HyperliquidDailyBalanceChange> balanceChangeRepository,
        IRepository<HyperliquidVaultPosition> positionRepository,
        ILogger<HyperliquidBalanceChangeService> logger) : base(balanceChangeRepository, logger)
    {
        _positionRepository = positionRepository;
    }

    public string Name => "Hyperliquid";

    protected override async Task<List<HyperliquidDailyBalanceChange>> GetDailyBalanceChangesAsync(
        IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to, CancellationToken ct)
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