using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.UniswapModule.Models;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public class LiquidityPoolEventDecoder : ILiquidityPoolEventDecoder
{
    private static readonly ParameterDecoder Decoder = new();

    private static readonly Parameter[] Parameters =
    [
        new("int24", 1), // tickLower
        new("int24", 2), // tickUpper
        new("int24", 3) // liquidityDelta
    ];

    public LiquidityPoolPositionEvent DecodeModifyLiquidityEvent(string fromAddress, string data,
        TokenPair tokenPair)
    {
        var decoded = Decoder.DecodeDefaultData(data, Parameters);

        return new LiquidityPoolPositionEvent
        {
            WalletAddress = fromAddress,
            TickLower = (BigInteger)decoded[0].Result,
            TickUpper = (BigInteger)decoded[1].Result,
            LiquidityDelta = (BigInteger)decoded[2].Result,
            TokenPair = tokenPair
        };
    }
}