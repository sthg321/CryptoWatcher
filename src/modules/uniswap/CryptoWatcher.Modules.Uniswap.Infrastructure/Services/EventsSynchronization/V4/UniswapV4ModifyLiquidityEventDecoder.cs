using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services.EventsSynchronization.V4;

internal class UniswapV4ModifyLiquidityEventDecoder : ILiquidityPoolEventDecoder
{
    private static readonly ParameterDecoder Decoder = new();

    private static readonly Parameter[] Parameters =
    [
        new("int24", 1), // tickLower
        new("int24", 2), // tickUpper
        new("int24", 3) // liquidityDelta
    ];

    public BlockchainLogType LogType => BlockchainLogType.ModifyLiquidity;

    public LiquidityPoolPositionEvent DecodeModifyLiquidityEvent(EvmAddress walletAddress,
        BlockchainLogEntry blockchainLogEntry,
        TokenPair tokenPair,
        DateTimeOffset timestamp)
    {
        var decoded = Decoder.DecodeDefaultData(blockchainLogEntry.Data, Parameters);

        return new LiquidityPoolPositionEvent
        {
            WalletAddress = walletAddress,
            TransactionHash = blockchainLogEntry.TransactionHash,
            TickLower = (BigInteger)decoded[0].Result,
            TickUpper = (BigInteger)decoded[1].Result,
            LiquidityDelta = (BigInteger)decoded[2].Result,
            TokenPair = tokenPair,
            TimeStamp = timestamp
        };
    }
}