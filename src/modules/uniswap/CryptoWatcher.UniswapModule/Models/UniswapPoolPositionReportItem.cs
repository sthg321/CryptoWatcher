using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.UniswapModule.Models;

/// <summary>
/// Represents a report item for a Uniswap pool position. This class contains details about
/// the pool position, including network, date, values in USD for position, hold, fees,
/// and the symbols of the token pair.
/// </summary>
public class UniswapPoolPositionReportItem
{
    /// <summary>
    /// Represents the blockchain network associated with a Uniswap pool or position (e.g., Ethereum, Binance Smart Chain).
    /// </summary>
    public required string Network { get; init; } = null!;

    /// <summary>
    /// Represents the specific day associated with a Uniswap pool position snapshot or report item.
    /// </summary>
    public required DateOnly Day { get; init; }

    /// <summary>
    /// Represents the total value of a position in the Uniswap liquidity pool, calculated in USD.
    /// </summary>
    public required Money PositionInUsd { get; init; }

    /// <summary>
    /// Represents the total value of tokens held in a Uniswap pool, calculated in US dollars.
    /// </summary>
    public required Money HoldInUsd { get; init; }

    /// <summary>
    /// Represents the total fee value in USD associated with a specific Uniswap pool position.
    /// This property is calculated based on the fee accumulations for a given pool position snapshot.
    /// </summary>
    public required Money FeeInUsd { get; init; }

    /// <summary>
    /// Represents the concatenated symbols of the token pair in a Uniswap pool, separated by a delimiter such as " / ".
    /// </summary>
    public required string TokenPairSymbols { get; init; } = null!;
}