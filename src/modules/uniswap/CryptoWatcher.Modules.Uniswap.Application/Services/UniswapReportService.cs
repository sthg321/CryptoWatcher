using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Extensions;
using CryptoWatcher.Models;
using CryptoWatcher.Modules.Merkl.Application.Abstractions;
using CryptoWatcher.Modules.Merkl.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapReportService : IPlatformDailyReportDataProvider
{
    private readonly IRepository<UniswapLiquidityPosition> _poolPositionRepository;
    private readonly IRewardService _rewardService;

    public UniswapReportService(IRepository<UniswapLiquidityPosition> poolPositionRepository,
        IRewardService rewardService)
    {
        _poolPositionRepository = poolPositionRepository;
        _rewardService = rewardService;
    }

    public async Task<PlatformDailyReportData> GetReportDataAsync(IReadOnlyCollection<Wallet> wallets, DateOnly from,
        DateOnly to, CancellationToken ct = default)
    {
        var poolPositions =
            await _poolPositionRepository.ListAsync(new UniswapPositionsForReportSpecification(wallets, from, to), ct);

        var result = new Dictionary<Wallet, List<PlatformDailyReport>>();
        foreach (var poolPositionByWallet in poolPositions.GroupBy(position => position.WalletAddress))
        {
            var rewards = (await _rewardService.GetUniswapRewardsAsync(poolPositionByWallet.Key, from, to, ct))
                .ToDictionary(reward => new UniswapReward.UniswapRewardKey(reward.NftId, reward.Day));

            foreach (var poolPosition in poolPositionByWallet.OrderBy(position => position.PositionId)
                         .ThenBy(position => position.IsClosed))
            {
                if (poolPosition.Snapshots.Count == 0)
                {
                    continue;
                }

                var profit = poolPosition.CalculateProfitInUsd(from, to);
                var report = new UniswapDailyReport
                {
                    PositionInUsd = poolPosition.Snapshots.MaxBy(snapshot => snapshot.Day)!.TokenSumInUsd(),
                    ProfitInUsd = profit.Amount,
                    ProfitInPercent = profit.Percent,
                    TotalHoldInUsd = poolPosition.CalculateHoldValueInUsd(to),
                    ReportItems = poolPosition.Snapshots.Select(positionSnapshot =>
                    {
                        return new UniswapDailyReportItem
                        {
                            Network = poolPosition.NetworkName,
                            Day = positionSnapshot.Day,
                            PositionInUsd = positionSnapshot.TokenSumInUsd(),
                            HoldInUsd = poolPosition.CalculateHoldValueInUsd(positionSnapshot.Day),
                            TokenPairSymbols = $"{poolPosition.Token0.Symbol} / {poolPosition.Token1.Symbol}",
                            DailyProfitInUsd = poolPosition.CalculateDailyFeeProfit(positionSnapshot.Day),
                            DailyProfitInUsdPercent = 0,
                            RewardsInUsd =
                                rewards.GetValueOrDefault(new UniswapReward.UniswapRewardKey(poolPosition.PositionId,
                                    positionSnapshot.Day))?.RewardsInUsd ?? 0
                        };
                    }).ToArray()
                };

                if (!result.TryGetValue(poolPosition.Wallet, out var dailyReports))
                {
                    result.Add(poolPosition.Wallet, dailyReports = []);
                }

                dailyReports.Add(report);
            }
        }

        return new PlatformDailyReportData
        {
            PlatformName = "Uniswap",
            Reports = result
        };
    }
}