using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using UniswapClient.Models;
using UniswapClient.UniswapV4.StateView;

namespace UniswapClient.UniswapV4;

public class UniswapV4PositionInfo : IUniswapPosition
{
    public required string Token0 { get; init; } = null!;
    
    public required string Token1 { get; init; } = null!;
    
    public required int TickLower { get; init; }
    
    public required int TickUpper { get; init; }
    
    public required BigInteger Liquidity { get; init; }
    
    public required BigInteger FeeGrowthInside0LastX128 { get; init; }
    
    public required BigInteger FeeGrowthInside1LastX128 { get; init; }
    
    public required BigInteger PositionId { get; init; }
    
    public int ProtocolVersion => 4;

    public required UniswapV4PoolKey PoolKey { get; init; } = null!;
}