using CryptoWatcher.Abstractions;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapDailyBalanceChangeService : BaseDailyBalanceChangeSynchronizer<UniswapDailyBalanceChange>,
    IDailyBalanceChangeSynchronizer
{
    private readonly IRepository<UniswapLiquidityPosition> _liquidityPositionRepository;
    
    public UniswapDailyBalanceChangeService(IRepository<UniswapDailyBalanceChange> balanceChangeRepository,
        IRepository<UniswapLiquidityPosition> liquidityPositionRepository,
        ILogger<BaseDailyBalanceChangeSynchronizer<UniswapDailyBalanceChange>> logger) : base(balanceChangeRepository,
        logger)
    {
        _liquidityPositionRepository = liquidityPositionRepository;
    }

    public string Name => "Uniswap";
 
    protected override async Task<List<UniswapDailyBalanceChange>> GetDailyBalanceChangesAsync(
        IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to, CancellationToken ct)
    {
        var positions = await _liquidityPositionRepository.ListAsync(
            new UniswapPositionsForReportSpecification(wallets, from, to), ct);

        var result = new List<UniswapDailyBalanceChange>();
        foreach (var uniswapLiquidityPosition in positions)
        {
            if (uniswapLiquidityPosition.PoolPositionSnapshots.Count == 0)
            {
                continue;
            }

            UniswapLiquidityPositionSnapshot? previousPositionSnapshot = null;

            foreach (var currentSnapshot in uniswapLiquidityPosition.PoolPositionSnapshots)
            {
                previousPositionSnapshot ??= currentSnapshot;

                var dailyBalanceChanged = UniswapDailyBalanceChange.Create(uniswapLiquidityPosition,
                    previousPositionSnapshot, currentSnapshot);

                result.Add(dailyBalanceChanged);
            }
        }

        return result;
    }
}