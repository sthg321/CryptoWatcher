using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.LiquidityPool.Contracts;

[FunctionOutput]
internal class FeeGrowthGlobalOutputDTO : IFunctionOutputDTO
{
    [Parameter("uint256", "", 1)] public BigInteger Value { get; set; }
}