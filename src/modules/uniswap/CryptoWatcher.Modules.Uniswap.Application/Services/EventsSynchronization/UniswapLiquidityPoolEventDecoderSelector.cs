using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.EventsSynchronization;

public class UniswapLiquidityPoolEventDecoderSelector : IUniswapLiquidityPoolEventDecoderSelector
{
    private readonly Dictionary<BlockchainLogType, ILiquidityPoolEventDecoder> _decoders;

    public UniswapLiquidityPoolEventDecoderSelector(IEnumerable<ILiquidityPoolEventDecoder> decoders)
    {
        _decoders = decoders.ToDictionary(x => x.LogType);
    }
    
    public LiquidityPoolPositionEvent DecodeEvent(EvmAddress walletAddress,
        BlockchainLogEntry blockchainLogEntry,
        TokenPair tokenPair, DateTimeOffset timestamp)
    {
        if (_decoders.TryGetValue(blockchainLogEntry.Type, out var eventDecoder))
        {
            return eventDecoder.DecodeModifyLiquidityEvent(walletAddress, blockchainLogEntry, tokenPair, timestamp);
        }
        
        throw new InvalidOperationException($"Can't decode event of type {blockchainLogEntry.Type}");
    }
}