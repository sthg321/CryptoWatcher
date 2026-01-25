using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;

public abstract class PositionOperation
{
    public required ulong PositionId { get; init; }

    public required TransactionHash TransactionHash { get; init; } = null!;
    
    public required Token Token0 { get; init; } = null!;

    public required Token Token1 { get; init; } = null!;
    
    public required BigInteger BlockNumber { get; init; }
}