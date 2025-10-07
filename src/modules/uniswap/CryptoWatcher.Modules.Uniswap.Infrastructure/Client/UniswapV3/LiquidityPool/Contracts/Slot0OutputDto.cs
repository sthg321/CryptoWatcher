using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace UniswapClient.UniswapV3.LiquidityPool.Contracts;

[FunctionOutput]
internal class Slot0OutputDto : IFunctionOutputDTO
{
    [Parameter("uint160", "sqrtPriceX96", 1)]
    public BigInteger SqrtPriceX96 { get; set; }

    [Parameter("int24", "tick", 2)] public int Tick { get; set; }
}