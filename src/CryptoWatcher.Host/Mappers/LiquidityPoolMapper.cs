using CryptoWatcher.Models;
using CryptoWatcher.UniswapModule.Models;
using Riok.Mapperly.Abstractions;
using UniswapClient.Models;

namespace CryptoWatcher.Host.Mappers;

[Mapper]
public static partial class LiquidityPoolMapper
{
    public static partial LiquidityPool MapToLiquidityPool(this LiquidityPoolInfo poolInfo);
}