using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.LiquidityPoolFactory.Contracts;

internal class PoolInfo
{
    [Parameter("address", "pool", 1)] public string Pool { get; set; } = null!;
}