namespace CryptoWatcher.UniswapModule.Entities;

public class PoolPositionSnapshot
{
    /// <summary>
    /// Represents the unique identifier for a liquidity pool position from NFT manager.
    /// </summary>
    /// <remarks>
    /// This property is used to uniquely identify a specific position within the liquidity pool.
    /// It serves as a key for referencing and managing individual positions across the system.
    /// </remarks>
    public ulong PoolPositionId { get; init; }

    /// <summary>
    /// Specifies the name of the uniswapNetwork associated with the current configuration or operation.
    /// </summary>
    /// <remarks>
    /// This property is used to identify the uniswapNetwork, such as a blockchain or communication uniswapNetwork,
    /// that the system is interacting with or utilizing for its processes.
    /// </remarks>
    public string NetworkName { get; init; } = null!;
    
    /// <summary>
    /// Represents the date when the liquidity pool position was created.
    /// </summary>
    /// <remarks>
    /// This property stores the creation date for a specific position in the liquidity pool.
    /// It is used to track the timeline of when the position was initialized.
    /// </remarks>
    public DateOnly Day { get; init; }
    
    public bool IsInRange { get; init; }

    public TokenInfoWithFee Token0 { get; set; } = null!;
    
    public TokenInfoWithFee Token1 { get; set; } = null!;
    
    public PoolPosition PoolPosition { get; init; } = null!;
    
    public decimal FeeInUsd => Token0.FeeAmount * Token0.PriceInUsd + Token1.FeeAmount * Token1.PriceInUsd;
}