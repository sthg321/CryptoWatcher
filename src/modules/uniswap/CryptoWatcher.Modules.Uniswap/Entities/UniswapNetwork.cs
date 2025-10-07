using CryptoWatcher.Modules.Uniswap.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Entities;

/// <summary>
/// Represents the configuration for a blockchain chain where the Uniswap protocol is deployed.
/// </summary>
/// <remarks>
/// This entity stores essential deployment details for Uniswap on a specific chain, including RPC endpoints,
/// core contract addresses, protocol version, and associated liquidity positions. It serves as a domain entity
/// for database persistence (via EF Core) and client initialization in the Infrastructure layer.
/// In DDD, this is part of the Uniswap Bounded Context, with Name as the aggregate root identifier.
/// </remarks>
public class UniswapChainConfiguration
{
    public required string Name { get; init; }= null!;

    public required string RpcUrl { get; init; } = null!;

    public required UniswapAddresses SmartContractAddresses { get; init; } = null!;

    public required UniswapProtocolVersion ProtocolVersion { get; init; }

    public IReadOnlyList<PoolPosition> LiquidityPoolPositions { get; init; } = [];
}