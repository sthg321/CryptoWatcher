namespace CryptoWatcher.Entities;

/// <summary>
/// Represents a cryptocurrency wallet entity within the system.
/// A wallet is defined by a unique identifier and an associated address.
/// </summary>
public class Wallet
{
    /// <summary>
    /// Gets the address associated with the wallet.
    /// This address is a unique identifier for blockchain interactions, allowing users or systems
    /// to send and receive cryptocurrency or interact with decentralized applications.
    /// </summary>
    public required string Address { get; init; } = null!;

    /// <summary>
    /// Gets the collection of liquidity pool positions associated with the wallet.
    /// Liquidity pool positions represent the user's participation in various liquidity pools,
    /// including details such as token amounts, equivalent USD values, and uniswapNetwork metadata.
    /// </summary>
    /// <remarks>
    /// This property provides access to all the liquidity pool positions that the wallet is involved in,
    /// allowing for tracking and analysis of investments across decentralized finance platforms.
    /// Each entry in the collection corresponds to a distinct position in a liquidity pool.
    /// </remarks>
    public List<LiquidityPoolPosition> LiquidityPoolPositions { get; init; } = [];
}