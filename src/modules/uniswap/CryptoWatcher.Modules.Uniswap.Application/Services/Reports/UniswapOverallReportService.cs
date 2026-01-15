using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Merkl.Application.Abstractions;
using CryptoWatcher.Modules.Merkl.Entities;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.Reports;
using CryptoWatcher.Modules.Uniswap.Application.Models.Reports;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Reports;

public class UniswapOverallReportService : BaseReportService<UniswapOverallReport>, IUniswapOverallReportService
{
    public UniswapOverallReportService(IRepository<UniswapLiquidityPosition> poolPositionRepository,
        IMerklRewardService merklRewardService) : base(poolPositionRepository, merklRewardService)
    {
    }

    protected override UniswapOverallReport CreateReportItem(UniswapLiquidityPosition position,
        MerklCampaign? merklCampaign, DateOnly from, DateOnly to)
    {
        var initialSnapshot = position.Snapshots.MinBy(snapshot => snapshot.Day);
        
        var report = new UniswapOverallReport
        {
            Network = position.NetworkName,
            Pair = position.TokenSymbols,
            CreatedAt = position.CreatedAt,
            ClosedAt = position.ClosedAt,
            InitialBalanceInUsd = initialSnapshot?.AmountInUsd ?? 0,
            CurrentBalanceInUsd = position.Snapshots.Last().AmountInUsd,
            CommissionInUsd = position.CalculateFeeForPeriod(from, to),
            RewardsInUsd = merklCampaign?.CalculateDailyRewardsInUsd(from, to) ?? 0,
        };

        return report;
    }
}