namespace CryptoWatcher.Entities;

/// <summary>
/// Represents the historical state of a specific liquidity pool position on a given day.
/// </summary>
/// <remarks>
/// This class is used to track daily snapshots of a liquidity pool position, including token balances,
/// their respective USD values, unclaimed fees, and APR. It is associated with a specific liquidity
/// pool position by a foreign key. Instances of this class are used for analytical and historical
/// performance purposes, offering insights into the performance and status of liquidity pool positions over time.
/// </remarks>
public class LiquidityPoolPositionSnapshot
{
    public DateOnly Day { get; init; }

    public string Token0Symbol { get; set; } = null!;
    public string Token1Symbol { get; set; } = null!;
    
    public decimal Token0Amount { get; init; }
    public decimal Token1Amount { get; init; }

    public decimal Token0PriceInUsd { get; init; }
    public decimal Token1PriceInUsd { get; init; }

    public decimal Token0FeesUnclaimed { get; init; }
    public decimal Token1FeesUnclaimed { get; init; }
  
    public bool IsInRange { get; init; }

    public ulong LiquidityPoolPositionId { get; init; }

    public string NetworkName { get; init; } = null!;

    public LiquidityPoolPosition LiquidityPoolPosition { get; init; } = null!;

    public decimal CalculateFeeInUsd()
    {
        return Token0FeesUnclaimed * Token0PriceInUsd +
               Token1FeesUnclaimed * Token1PriceInUsd;
    }
}