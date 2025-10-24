using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapDailyBalanceChangeService : IUniswapDailyBalanceChangeService
{
    private readonly IRepository<UniswapLiquidityPosition> _liquidityPositionRepository;

    public UniswapDailyBalanceChangeService(IRepository<UniswapLiquidityPosition> liquidityPositionRepository)
    {
        _liquidityPositionRepository = liquidityPositionRepository;
    }

    public async Task<List<UniswapDailyBalanceChange>> GetDailyBalanceChangeAsync(IReadOnlyCollection<Wallet> wallets,
        DateOnly from, DateOnly to,
        CancellationToken ct = default)
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