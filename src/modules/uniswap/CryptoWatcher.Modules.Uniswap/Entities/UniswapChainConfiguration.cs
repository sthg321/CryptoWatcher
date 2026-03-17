using System.Numerics;
using CryptoWatcher.Modules.Uniswap.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Entities;

/// <summary>
/// Represents the configuration for a blockchain chain where the Uniswap protocol is deployed.
/// </summary>
/// <remarks>
/// This entity stores essential deployment details for Uniswap on a specific chain, including RPC endpoints,
/// core contract addresses, protocol version, and associated liquidity positions.
/// </remarks>
public class UniswapChainConfiguration : BaseChainConfiguration
{
    // ReSharper disable once CollectionNeverUpdated.Local
    private readonly List<UniswapLiquidityPosition> _liquidityPoolPositions = [];
    
    /// <summary>
    /// Gets the identifier of the blockchain network where the Uniswap protocol is deployed.
    /// </summary>
    /// <remarks>
    /// This property represents the chain ID of a specific network, which is used to distinguish
    /// between multiple blockchain networks within the Uniswap protocol's configurations. It is
    /// critical for ensuring protocol operations and interactions are performed on the correct
    /// blockchain network.
    /// </remarks>
    public required int ChainId { get; init; }
    
    /// <summary>
    /// Gets the URL of the Blockscout instance associated with the blockchain chain.
    /// </summary>
    /// <remarks>
    /// This property provides the endpoint for accessing blockchain data and analytics through Blockscout,
    /// which is a block explorer and analytics platform. It is used for various service queries and data retrievals
    /// within the application, such as fetching transaction details or blockchain state.
    /// </remarks>
    public required Uri BlockscoutUrl { get; init; }
    
    /// <summary>
    /// Represents the required collection of smart contract addresses specific
    /// to the configuration of an Uniswap chain, defining addresses for key
    /// on-chain parts and operations.
    /// </summary>
    public required UniswapAddresses SmartContractAddresses { get; init; } = null!;

    /// <summary>
    /// Denotes the specific version of the Uniswap protocol associated with the chain configuration
    /// and dictates the behavior and features available based on the version defined.
    /// </summary>
    public required UniswapProtocolVersion ProtocolVersion { get; init; }

    /// <summary>
    /// The last block number that has been successfully processed for Uniswap data
    /// </summary>
    public BigInteger LastProcessedBlock { get; init; }

    /// <summary>
    /// Timestamp when the synchronization state was last updated
    /// </summary>
    public DateTime LastProcessedBlockUpdatedAt { get; private set; } = DateTime.UtcNow;

    public IReadOnlyCollection<UniswapLiquidityPosition> LiquidityPoolPositions => _liquidityPoolPositions;
    
    public IReadOnlyCollection<UniswapAddresses> SmartContractAddressesList => [SmartContractAddresses];
}