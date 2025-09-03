using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.UniswapModule.Models;

/// <summary>
/// Represents a report that summarizes the position of a Uniswap pool.
/// This includes information such as the total value of the pool in USD, total holding in USD,
/// total fees earned in USD, and a collection of individual pool position details.
/// </summary>
public class UniswapPoolPositionReport
{
    /// <summary>
    /// Gets the total position value in USD derived from the Uniswap pool positions.
    /// This value represents the cumulative monetary value of all token positions within the pools.
    /// </summary>
    public required Money TotalPositionInUsd { get; init; }

    /// <summary>
    /// Gets the total hold value in USD derived from the Uniswap pool positions.
    /// This value represents the aggregate monetary value of the tokens held in the pool positions.
    /// </summary>
    public required Money TotalHoldInUsd { get; init; }

    /// <summary>
    /// Gets the total fee value in USD derived from the Uniswap pool positions.
    /// This includes the sum of all fees earned across all snapshots of the pool positions.
    /// </summary>
    public required Money TotalFeeInUsd { get; init; }

    /// <summary>
    /// Gets the collection of report items that represent individual snapshots of a Uniswap pool position.
    /// Each report item includes detailed information such as the network, date, token pair,
    /// position values in USD, hold values in USD, and fee values in USD.
    /// </summary>
    public required IReadOnlyCollection<UniswapPoolPositionReportItem> ReportItems { get; init; } = [];
}