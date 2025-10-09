using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Abstractions;

public interface ILiquidityPoolEventDecoder
{
    LiquidityPoolPositionEvent DecodeModifyLiquidityEvent(string walletAddress, string data,
        string transactionHash,
        TokenPair tokenPair,
        DateTimeOffset timestamp);
}