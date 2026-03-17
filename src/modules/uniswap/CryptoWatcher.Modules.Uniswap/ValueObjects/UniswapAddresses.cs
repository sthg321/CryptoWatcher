using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.ValueObjects;

/// <summary>
/// Represents the configured smart contract addresses used within the Uniswap module.
/// This record object provides essential contract addresses required for interacting
/// with the Uniswap protocol on specific blockchain networks.
/// </summary>
public record UniswapAddresses
{
    /// <summary>
    /// Represents the address of the Position Manager contract in the Uniswap ecosystem.
    /// </summary>
    /// <remarks>
    /// The Position Manager is a core smart contract in Uniswap protocol, providing functionality
    /// for managing user positions, such as adding liquidity or modifying existing liquidity.
    /// </remarks>
    /// <value>
    /// An instance of <see cref="EvmAddress"/> representing the Ethereum address of the Position Manager contract.
    /// </value>
    public required EvmAddress PositionManager { get; init; } = null!;

    /// <summary>
    /// Represents the address of the Pool Factory smart contract in Uniswap.
    /// This property is used for accessing and interacting with the Pool Factory
    /// to facilitate the creation of liquidity pools and related operations.
    /// </summary>
    /// <remarks>
    /// The Pool Factory acts as a registry for managing and deploying liquidity pools
    /// on the Uniswap protocol. This address is chain-specific and must be appropriately
    /// configured for each supported blockchain network.
    /// </remarks>
    public required EvmAddress PoolFactory { get; init; } = null!;

    /// <summary>
    /// Represents the address of the MultiCall smart contract used for batch function calls on the blockchain.
    /// This property is integral for efficiently aggregating and executing multiple read-only contract calls
    /// in a single transaction, reducing the number of network interactions.
    /// </summary>
    /// <remarks>
    /// The MultiCall contract is often used in services requiring information from multiple contracts or
    /// repeated calls to a single contract. It helps enhance performance by enabling the aggregation of these calls.
    /// The address is specific to the blockchain network being used and must correspond to a deployed MultiCall contract
    /// on that network.
    /// </remarks>
    public required EvmAddress MultiCall { get; init; } = null!;

    public EvmAddress? StateView { get; set; }

    public UniswapProtocolVersion ProtocolVersion { get; set; } = UniswapProtocolVersion.V3;
}