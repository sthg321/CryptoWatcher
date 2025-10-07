using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace UniswapClient.UniswapV3.PositionsFetcher.Contracts;

[FunctionOutput]
public class PositionsOutputDTO : IFunctionOutputDTO
{
    [Parameter("uint96", "nonce", 1)] public BigInteger Nonce { get; set; }
    
    [Parameter("address", "operator", 2)] public string Operator { get; set; } = null!;
    
    [Parameter("address", "token0", 3)] public string Token0 { get; set; } = null!;
    
    [Parameter("address", "token1", 4)] public string Token1 { get; set; } = null!;
    
    [Parameter("uint24", "fee", 5)] public uint Fee { get; set; }
    
    [Parameter("int24", "tickLower", 6)] public int TickLower { get; set; }
    
    [Parameter("int24", "tickUpper", 7)] public int TickUpper { get; set; }
    
    [Parameter("uint128", "liquidity", 8)] public BigInteger Liquidity { get; set; }

    [Parameter("uint256", "feeGrowthInside0LastX128", 9)]
    public BigInteger FeeGrowthInside0LastX128 { get; set; }

    [Parameter("uint256", "feeGrowthInside1LastX128", 10)]
    public BigInteger FeeGrowthInside1LastX128 { get; set; }
}