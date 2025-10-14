using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.LiquidityPool.Contracts;

[Function("feeGrowthGlobal0X128", "uint256")]
internal class FeeGrowthGlobal0X128Function : FunctionMessage;