using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.UniswapModule.Models;
using CryptoWatcher.UniswapModule.Specifications;

namespace CryptoWatcher.UniswapModule.Services;

/// <summary>
/// <see cref="IUniswapReportService"/>
/// </summary>
internal class UniswapReportService : IPlatformDailyReportDataProvider
{
    private readonly IRepository<PoolPosition> _poolPositionRepository;

    public UniswapReportService(IRepository<PoolPosition> poolPositionRepository)
    {
        _poolPositionRepository = poolPositionRepository;
    }

    public async Task<PlatformDailyReportData> GetReportDataAsync(IReadOnlyCollection<Wallet> wallets, DateOnly from,
        DateOnly to, CancellationToken ct = default)
    {
        var poolPositions =
            await _poolPositionRepository.ListAsync(new UniswapPositionsForReportSpecification(wallets, from, to), ct);

        var result = new Dictionary<Wallet, List<PlatformDailyReport>>();
        foreach (var poolPositionByWallet in poolPositions.GroupBy(position => position.WalletAddress))
        {
            foreach (var poolPosition in poolPositionByWallet)
            {
                var report = new UniswapDailyReport
                {
                    PositionInUsd = poolPositions
                        .Select(static position => position.PoolPositionSnapshots.MaxBy(snapshot => snapshot.Day))
                        .Sum(static snapshot => snapshot!.TokenSumInUsd()),
                    ProfitInUsd =
                        poolPositions.Sum(static position => CalculateActualFee(position.PoolPositionSnapshots)),
                    ProfitInPercent = 0,
                    TotalHoldInUsd = poolPositions
                        .Sum(static position =>
                        {
                            var lastPosition =
                                position.PoolPositionSnapshots.MaxBy(positionSnapshot => positionSnapshot.Day);

                            return position.Token0.Amount * lastPosition!.Token0.PriceInUsd +
                                   position.Token1.Amount * lastPosition.Token1.PriceInUsd;
                        }),

                    ReportItems = poolPosition.PoolPositionSnapshots.Select(positionSnapshot =>
                        new UniswapDailyReportItem
                        {
                            Network = poolPosition.NetworkName,
                            Day = positionSnapshot.Day,
                            PositionInUsd = positionSnapshot.Token0.AmountInUsd + positionSnapshot.Token1.AmountInUsd,
                            HoldInUsd = poolPosition.Token0.Amount * positionSnapshot.Token0.PriceInUsd +
                                        poolPosition.Token1.Amount * positionSnapshot.Token1.PriceInUsd,
                            TokenPairSymbols = $"{positionSnapshot.Token0.Symbol} / {positionSnapshot.Token0.Symbol}",
                            DailyProfitInUsd = positionSnapshot.FeeInUsd,
                            DailyProfitInUsdPercent = 0
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

    /// <summary>
    /// Calculate actual fee from cumulative fee.
    /// </summary>
    /// <param name="positionFees"></param>
    /// <returns></returns>
    private static decimal CalculateActualFee(IEnumerable<PoolPositionSnapshot> positionFees)
    {
        var prevDayFee = 0m;
        var result = 0m;
        foreach (var poolPositionFee in positionFees.OrderBy(snapshot => snapshot.Day))
        {
            if (poolPositionFee.FeeInUsd > prevDayFee)
            {
                prevDayFee = poolPositionFee.FeeInUsd;
                continue;
            }

            if (poolPositionFee.FeeInUsd < prevDayFee)
            {
                result += prevDayFee;
                prevDayFee = 0;
            }
        }

        return result + prevDayFee;
    }
}