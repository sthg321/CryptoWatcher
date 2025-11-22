using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface ILiquidityPoolEventDecoder
{
    BlockchainLogType LogType { get; }
    
    LiquidityPoolPositionEvent DecodeModifyLiquidityEvent(EvmAddress walletAddress,
        BlockchainLogEntry blockchainLogEntry,
        TokenPair tokenPair,
        DateTimeOffset timestamp);
}