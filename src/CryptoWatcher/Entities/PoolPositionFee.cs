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
public class PoolPositionFee
{
    public DateOnly Day { get; init; }

    public TokenInfo Token0Fee { get; init; } = null!;
    
    public TokenInfo Token1Fee { get; init; } = null!;
    
    public bool IsInRange { get; init; }

    public ulong LiquidityPoolPositionId { get; init; }

    public string NetworkName { get; init; } = null!;

    public PoolPosition PoolPosition { get; init; } = null!;

    public decimal CalculateFeeInUsd()
    {
        return Token0Fee.AmountInUsd + Token1Fee.AmountInUsd;
    }
}