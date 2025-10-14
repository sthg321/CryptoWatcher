using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services.EventsSynchronization;

internal class LiquidityPoolEventDecoder : ILiquidityPoolEventDecoder
{
    private static readonly ParameterDecoder Decoder = new();

    private static readonly Parameter[] Parameters =
    [
        new("int24", 1), // tickLower
        new("int24", 2), // tickUpper
        new("int24", 3) // liquidityDelta
    ];

    public LiquidityPoolPositionEvent DecodeModifyLiquidityEvent(EvmAddress walletAddress, string data,
        TransactionHash transactionHash,
        TokenPair tokenPair,
        DateTimeOffset timestamp)
    {
        var decoded = Decoder.DecodeDefaultData(data, Parameters);

        return new LiquidityPoolPositionEvent
        {
            WalletAddress = walletAddress,
            TransactionHash = transactionHash,
            TickLower = (BigInteger)decoded[0].Result,
            TickUpper = (BigInteger)decoded[1].Result,
            LiquidityDelta = (BigInteger)decoded[2].Result,
            TokenPair = tokenPair,
            TimeStamp = timestamp
        };
    }
}