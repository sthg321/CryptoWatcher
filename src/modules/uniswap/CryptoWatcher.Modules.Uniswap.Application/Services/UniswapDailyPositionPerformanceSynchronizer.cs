using CryptoWatcher.Abstractions;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapDailyPositionPerformanceSynchronizer :
    BaseDailyPositionPerformanceSynchronizer<UniswapPositionDailyPerformance>,
    IDailyPositionPerformanceSynchronizer
{
    private readonly IRepository<UniswapLiquidityPosition> _liquidityPositionRepository;

    public UniswapDailyPositionPerformanceSynchronizer(
        IRepository<UniswapPositionDailyPerformance> balanceChangeRepository,
        IRepository<UniswapLiquidityPosition> liquidityPositionRepository,
        ILogger<UniswapDailyPositionPerformanceSynchronizer> logger) : base(balanceChangeRepository, logger)
    {
        _liquidityPositionRepository = liquidityPositionRepository;
    }

    public string Name => "Uniswap";

    protected override async Task<List<UniswapPositionDailyPerformance>> GetDailyBalanceChangesAsync(
        IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to, CancellationToken ct)
    {
        var positions = await _liquidityPositionRepository.ListAsync(
            new UniswapPositionsForReportSpecification(wallets, from, to), ct);

        var result = new List<UniswapPositionDailyPerformance>();
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

                var dailyBalanceChanged = UniswapPositionDailyPerformance.Create(uniswapLiquidityPosition,
                    previousPositionSnapshot, currentSnapshot);

                result.Add(dailyBalanceChanged);
            }
        }

        return result;
    }
}