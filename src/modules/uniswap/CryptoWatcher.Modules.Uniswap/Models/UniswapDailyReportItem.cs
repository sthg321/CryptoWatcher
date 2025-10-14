using CryptoWatcher.Models;
using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Models;

/// <summary>
/// Represents a report item for a Uniswap pool position. This class contains details about
/// the pool position, including network, date, values in USD for position, hold, fees,
/// and the symbols of the token pair.
/// </summary>
public class UniswapDailyReportItem : PlatformDailyReportItem
{
    /// <summary>
    /// Represents the blockchain network associated with a Uniswap pool or position (e.g., Ethereum, Binance Smart Chain).
    /// </summary>
    public required string Network { get; init; } = null!;
 
    /// <summary>
    /// Represents the total value of tokens held in a Uniswap pool, calculated in US dollars.
    /// </summary>
    public required Money HoldInUsd { get; init; }

    /// <summary>
    /// Represents the concatenated symbols of the token pair in a Uniswap pool, separated by a delimiter such as " / ".
    /// </summary>
    public required string TokenPairSymbols { get; init; } = null!;
}