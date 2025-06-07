using System.Numerics;
using UniswapClient.Models;

namespace UniswapClient.UniswapV3;

/// <summary>
/// <see cref="IUniswapPosition"/>
/// </summary>
public class UniswapV3PositionInfo : IUniswapPosition
{
    public required string Token0 { get; init; } = null!;
    
    public required string Token1 { get; init; } = null!;
    
    public required int TickLower { get; init; }
    
    public required int TickUpper { get; init; }
    
    public required BigInteger Liquidity { get; init; }
    
    public required BigInteger FeeGrowthInside0LastX128 { get; init; }
    
    public required BigInteger FeeGrowthInside1LastX128 { get; init; }
    
    public required BigInteger PositionId { get; init; }

    public int ProtocolVersion => 3;
}