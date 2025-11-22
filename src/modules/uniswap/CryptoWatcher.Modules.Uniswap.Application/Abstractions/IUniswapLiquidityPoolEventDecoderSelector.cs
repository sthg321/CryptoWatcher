using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapLiquidityPoolEventDecoderSelector
{
    LiquidityPoolPositionEvent DecodeEvent(EvmAddress walletAddress,
        BlockchainLogEntry blockchainLogEntry,
        TokenPair tokenPair, DateTimeOffset timestamp);
}