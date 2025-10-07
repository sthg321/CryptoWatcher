using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.UniswapModule.Models;

namespace CryptoWatcher.Modules.Uniswap.Abstractions;

public interface ILiquidityPoolEventDecoder
{
    LiquidityPoolPositionEvent DecodeModifyLiquidityEvent(string fromAddress, string data,
        TokenPair tokenPair);
}