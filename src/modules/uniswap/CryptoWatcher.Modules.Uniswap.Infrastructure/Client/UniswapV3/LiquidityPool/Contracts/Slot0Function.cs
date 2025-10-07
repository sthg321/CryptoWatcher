using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace UniswapClient.UniswapV3.LiquidityPool.Contracts;

[Function("slot0", typeof(Slot0OutputDto))]
internal class Slot0Function : FunctionMessage
{
}