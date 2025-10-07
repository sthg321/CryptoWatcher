using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace UniswapClient.UniswapV3.LiquidityPool.Contracts;

[Function("feeGrowthGlobal0X128", "uint256")]
internal class FeeGrowthGlobal0X128Function : FunctionMessage
{
}