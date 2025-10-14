using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Abstractions;

public interface ILiquidityPoolEventDecoder
{
    LiquidityPoolPositionEvent DecodeModifyLiquidityEvent(EvmAddress walletAddress, string data,
        TransactionHash transactionHash,
        TokenPair tokenPair,
        DateTimeOffset timestamp);
}