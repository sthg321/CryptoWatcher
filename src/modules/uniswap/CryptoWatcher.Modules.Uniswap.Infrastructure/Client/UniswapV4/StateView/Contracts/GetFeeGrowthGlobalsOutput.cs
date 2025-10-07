using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace UniswapClient.UniswapV4.StateView.Contracts;

[FunctionOutput]
public class GetFeeGrowthGlobalsOutput : IFunctionOutputDTO
{
    [Parameter("uint256", "feeGrowthGlobal0", 1)]
    public BigInteger FeeGrowthGlobal0 { get; set; }

    [Parameter("uint256", "feeGrowthGlobal1", 2)]
    public BigInteger FeeGrowthGlobal1 { get; set; }
}