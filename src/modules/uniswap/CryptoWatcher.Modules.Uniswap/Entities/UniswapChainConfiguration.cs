using System.Numerics;
using CryptoWatcher.Exceptions;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Modules.Uniswap.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Entities;

/// <summary>
/// Represents the configuration for a blockchain chain where the Uniswap protocol is deployed.
/// </summary>
/// <remarks>
/// This entity stores essential deployment details for Uniswap on a specific chain, including RPC endpoints,
/// core contract addresses, protocol version, and associated liquidity positions.
/// </remarks>
public class UniswapChainConfiguration
{
    // ReSharper disable once CollectionNeverUpdated.Local
    private readonly List<UniswapLiquidityPosition> _liquidityPoolPositions = [];

    /// <summary>
    /// Gets the unique identifier or name of the blockchain chain where the Uniswap protocol is deployed.
    /// </summary>
    /// <remarks>
    /// This property is used to distinguish between different Uniswap chain configurations
    /// and serves as a reference in various operations and database mappings.
    /// </remarks>
    public required string Name { get; init; } = null!;

    /// <summary>
    /// Gets the URL of the RPC (Remote Procedure Call) endpoint for the blockchain network.
    /// </summary>
    /// <remarks>
    /// This property specifies the RPC endpoint used to interact with the blockchain network
    /// for executing transactions, querying data, and other interactions required by the Uniswap protocol.
    /// </remarks>
    public required Uri RpcUrl { get; init; } = null!;
    
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
    /// Gets the authorization token for the RPC endpoint (e.g., API key for Infura, RPC).
    /// </summary>
    /// <remarks>
    /// Stored separately for security; can be rotated independently. Do not log or expose in APIs.
    /// </remarks>
    public string? RpcAuthToken { get; init; } // store later with encryption

    /// <summary>
    /// Gets a composed URL that includes the RPC base URL and the optional authentication token.
    /// </summary>
    /// <remarks>
    /// This property concatenates the base RPC URL with the authentication token, if provided.
    /// It is used to authenticate requests to the blockchain RPC endpoint for interacting with
    /// the Uniswap protocol on a specific chain.
    /// </remarks>
    public string RpcUrlWithAuthToken => RpcAuthToken is not null ? $"{RpcUrl}/{RpcAuthToken}" : RpcUrl.ToString();

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
    public BigInteger LastProcessedBlock { get; set; }

    /// <summary>
    /// Timestamp when the synchronization state was last updated
    /// </summary>
    public DateTime LastProcessedBlockUpdatedAt { get; private set; } = DateTime.UtcNow;

    public IReadOnlyCollection<UniswapLiquidityPosition> LiquidityPoolPositions => _liquidityPoolPositions;

    public void UpdateLastSynchronizedBlock(BigInteger lastSynchronizedBlock)
    {
        if (lastSynchronizedBlock <= LastProcessedBlock)
        {
            throw new DomainException("Last processed block cannot be less than or equal to the current block");
        }

        LastProcessedBlock = lastSynchronizedBlock;
        LastProcessedBlockUpdatedAt = DateTime.UtcNow;
    }
}