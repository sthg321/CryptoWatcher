using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace UniswapClient.UniswapV3.LiquidityPool.Contracts;

[Function("feeGrowthGlobal1X128", "uint256")]
internal class FeeGrowthGlobal1X128Function : FunctionMessage
{
}
