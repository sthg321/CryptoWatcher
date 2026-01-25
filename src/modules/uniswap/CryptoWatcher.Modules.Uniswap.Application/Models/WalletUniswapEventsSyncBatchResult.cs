using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public record WalletUniswapEventsSyncBatchResult
{
    public required UniswapLiquidityPosition[] UpdatedPositions { get; init; }

    public required UniswapEvent LastEvent { get; init; }
}