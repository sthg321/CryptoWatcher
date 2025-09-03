using CryptoWatcher.Abstractions;
using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.UniswapModule.Models;
using CryptoWatcher.UniswapModule.Specifications;

namespace CryptoWatcher.UniswapModule.Services;

/// <summary>
/// Interface that defines methods for generating reports for Uniswap pool positions.
/// </summary>
public interface IUniswapReportService
{
    /// <summary>
    /// Creates a report for Uniswap pool positions within a specified date range.
    /// </summary>
    /// <param name="from">The start date of the report interval.</param>
    /// <param name="to">The end date of the report interval.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of Uniswap pool position reports.</returns>
    Task<List<UniswapPoolPositionReport>> CreateReportAsync(DateOnly from, DateOnly to,
        CancellationToken ct = default);
}

/// <summary>
/// <see cref="IUniswapReportService"/>
/// </summary>
internal class UniswapReportService : IUniswapReportService
{
    private readonly IRepository<PoolPosition> _poolPositionRepository;

    public UniswapReportService(IRepository<PoolPosition> poolPositionRepository)
    {
        _poolPositionRepository = poolPositionRepository;
    }

    public async Task<List<UniswapPoolPositionReport>> CreateReportAsync(DateOnly from, DateOnly to,
        CancellationToken ct = default)
    {
        var poolPositions =
            await _poolPositionRepository.ListAsync(new UniswapPositionsForReportSpecification(from, to), ct);

        var result = new List<UniswapPoolPositionReport>(poolPositions.Count);
        foreach (var poolPosition in poolPositions)
        {
            var report = new UniswapPoolPositionReport
            {
                TotalPositionInUsd = poolPositions
                    .Select(static position => position.PoolPositionSnapshots.MaxBy(snapshot => snapshot.Day))
                    .Sum(static snapshot => snapshot!.TokenSumInUsd()),
                TotalFeeInUsd =
                    poolPositions.Sum(static position => CalculateActualFee(position.PoolPositionSnapshots)),
                TotalHoldInUsd = poolPositions
                    .Sum(static position =>
                    {
                        var lastPosition =
                            position.PoolPositionSnapshots.MaxBy(positionSnapshot => positionSnapshot.Day);

                        return position.Token0.Amount * lastPosition!.Token0.PriceInUsd +
                               position.Token1.Amount * lastPosition.Token1.PriceInUsd;
                    }),
                ReportItems = poolPosition.PoolPositionSnapshots.Select(positionSnapshot =>
                    new UniswapPoolPositionReportItem
                    {
                        Network = poolPosition.NetworkName,
                        Day = positionSnapshot.Day,
                        PositionInUsd = positionSnapshot.Token0.AmountInUsd + positionSnapshot.Token1.AmountInUsd,
                        HoldInUsd = poolPosition.Token0.Amount * positionSnapshot.Token0.PriceInUsd +
                                    poolPosition.Token1.Amount * positionSnapshot.Token1.PriceInUsd,
                        TokenPairSymbols = $"{positionSnapshot.Token0.Symbol} / {positionSnapshot.Token0.Symbol}",
                        FeeInUsd = positionSnapshot.FeeInUsd,
                    }).ToArray()
            };

            result.Add(report);
        }

        return result;
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