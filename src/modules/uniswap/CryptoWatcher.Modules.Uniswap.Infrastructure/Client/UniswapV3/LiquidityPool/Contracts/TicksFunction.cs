using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace UniswapClient.UniswapV3.LiquidityPool.Contracts;

[Function("ticks", typeof(TickInfo))]
internal class TicksFunction : FunctionMessage
{
    [Parameter("int24", "tick")] public int Tick { get; set; }
}