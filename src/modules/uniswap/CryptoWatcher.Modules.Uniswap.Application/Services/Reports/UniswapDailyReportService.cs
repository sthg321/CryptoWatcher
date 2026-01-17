using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Extensions;
using CryptoWatcher.Models;
using CryptoWatcher.Modules.Merkl.Application.Abstractions;
using CryptoWatcher.Modules.Merkl.Entities;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Reports;

public class UniswapDailyReportService : BaseReportService<PlatformDailyReport>, IPlatformDailyReportDataProvider
{
    public UniswapDailyReportService(IRepository<UniswapLiquidityPosition> poolPositionRepository,
        IMerklRewardService merklRewardService) : base(poolPositionRepository, merklRewardService)
    {
    }

    public new async Task<PlatformDailyReportData> GetReportDataAsync(IReadOnlyCollection<Wallet> wallets,
        DateOnly from,
        DateOnly to, CancellationToken ct = default)
    {
        var result = await base.GetReportDataAsync(wallets, from, to, ct);

        return new PlatformDailyReportData
        {
            PlatformName = "Uniswap",
            Reports = result
        };
    }

    protected override UniswapDailyReport CreateReportItem(UniswapLiquidityPosition position,
        MerklCampaign? merklCampaign, DateOnly from,
        DateOnly to)
    {
        var profit = position.CalculateProfitInUsd(from, to);
        var report = new UniswapDailyReport
        {
            PositionInUsd = position.Snapshots.MaxBy(snapshot => snapshot.Day)!.TokenSumInUsd(),
            ProfitInUsd = profit.Amount,
            ProfitInPercent = profit.Percent,
            TotalHoldInUsd = position.CalculateHoldValueInUsd(to),
            ReportItems = position.Snapshots.Select(positionSnapshot =>
            {
                return new UniswapDailyReportItem
                {
                    Network = position.NetworkName,
                    Day = positionSnapshot.Day,
                    PositionInUsd = positionSnapshot.TokenSumInUsd(),
                    HoldInUsd = position.CalculateHoldValueInUsd(positionSnapshot.Day),
                    TokenPairSymbols = position.TokenSymbols,
                    DailyProfitInUsd = position.CalculateDailyFeeProfit(positionSnapshot.Day),
                    DailyProfitInUsdPercent = 0,
                    RewardsInUsd = merklCampaign?.CalculateDailyRewardsInUsd(positionSnapshot.Day) ?? 0
                };
            }).ToArray()
        };

        return report;
    }
}