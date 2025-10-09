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
    private readonly List<PoolPosition> _liquidityPoolPositions = [];
    
    public required string Name { get; init; } = null!;

    public required string RpcUrl { get; init; } = null!;

    public required UniswapAddresses SmartContractAddresses { get; init; } = null!;

    public required UniswapProtocolVersion ProtocolVersion { get; init; }

    /// <summary>
    /// The last block number that has been successfully processed for Uniswap data
    /// </summary>
    public BigInteger LastProcessedBlock { get;   set; }

    /// <summary>
    /// Timestamp when the synchronization state was last updated
    /// </summary>
    public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

    public IReadOnlyCollection<PoolPosition> LiquidityPoolPositions => _liquidityPoolPositions;

    public void UpdateLastSynchronizedBlock(BigInteger lastSynchronizedBlock)
    {
        if (lastSynchronizedBlock <= LastProcessedBlock)
        {
            throw new DomainException("Last processed block cannot be less than or equal to the current block");
        }

        LastProcessedBlock = lastSynchronizedBlock;
        LastUpdated = DateTime.UtcNow;
    }
}