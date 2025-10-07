using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace UniswapClient.UniswapV4.StateView.Contracts;

[FunctionOutput]
public class GetPositionInfoOutputDTO : IFunctionOutputDTO
{
    [Parameter("uint128", "liquidity", 1)] public BigInteger Liquidity { get; set; }

    [Parameter("uint256", "feeGrowthInside0LastX128", 2)]
    public BigInteger FeeGrowthInside0LastX128 { get; set; }

    [Parameter("uint256", "feeGrowthInside1LastX128", 3)]
    public BigInteger FeeGrowthInside1LastX128 { get; set; }
}