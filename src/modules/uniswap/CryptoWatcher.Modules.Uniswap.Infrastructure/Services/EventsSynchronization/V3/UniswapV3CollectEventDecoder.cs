using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services.EventsSynchronization.V3;

public class UniswapV3CollectEventDecoder : ILiquidityPoolEventDecoder
{
    private static readonly ParameterDecoder Decoder = new();

    public BlockchainLogType LogType => BlockchainLogType.Collect;

    public LiquidityPoolPositionEvent DecodeModifyLiquidityEvent(EvmAddress walletAddress,
        BlockchainLogEntry blockchainLogEntry,
        TokenPair tokenPair, DateTimeOffset timestamp)
    {
        var tickLower = Decoder.DecodeDefaultData(blockchainLogEntry.Topics[2].ToString(), new Parameter("int24"))
            .FirstOrDefault();

        if (tickLower is null)
        {
            throw new InvalidOperationException("Can't decode tickLower");
        }

        var tickUpper = Decoder.DecodeDefaultData(blockchainLogEntry.Topics[3].ToString(), new Parameter("int24"))
            .FirstOrDefault();

        if (tickUpper is null)
        {
            throw new InvalidOperationException("Can't decode tickUpper");
        }
        
        return new LiquidityPoolPositionEvent
        {
            WalletAddress = walletAddress,
            TransactionHash = blockchainLogEntry.TransactionHash,
            TickLower = BigInteger.Parse(tickLower.Result.ToString()!),
            TickUpper = BigInteger.Parse(tickUpper.Result.ToString()!),
            LiquidityDelta = 0,
            TokenPair = tokenPair,
            TimeStamp = timestamp
        };
    }
}