using CryptoWatcher.Modules.Merkl.Application.Abstractions;
using CryptoWatcher.Modules.Merkl.Entities;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.Reports;
using CryptoWatcher.Modules.Uniswap.Application.Models.Reports;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Reports;

public class UniswapOverallReportService : BaseReportService<UniswapOverallReport>, IUniswapOverallReportService
{
    public UniswapOverallReportService(IUniswapLiquidityPositionRepository poolPositionRepository,
        IMerklRewardService merklRewardService) : base(merklRewardService, poolPositionRepository)
    {
    }


    protected override UniswapOverallReport CreateReportItem(UniswapLiquidityPosition position,
        MerklCampaign? merklCampaign, DateOnly from, DateOnly to)
    {
        var report = new UniswapOverallReport
        {
            Network = position.NetworkName,
            Pair = position.TokenSymbols,
            CreatedAt = position.CreatedAt,
            ClosedAt = position.ClosedAt,
            InitialBalanceInUsd = position.CalculateInitialAmountInUsd(),
            CurrentBalanceInUsd = position.CalculateCurrentAmountInUsd(),
            CommissionInUsd = position.CalculateFeeForPeriod(from, to),
            RewardsInUsd = merklCampaign?.CalculateDailyRewardsInUsd(from, to) ?? 0,
        };

        return report;
    }
}