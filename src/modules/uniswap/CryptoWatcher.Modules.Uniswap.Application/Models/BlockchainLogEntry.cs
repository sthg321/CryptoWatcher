using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public record BlockchainLogEntry
{
    public required TransactionHash TransactionHash { get; init; } = null!;

    public required string Data { get; init; } = null!;

    public required EvmAddress Address { get; init; } = null!;

    public required object[] Topics { get; init; } = null!;

    public BlockchainLogType Type => DetectType();

    private BlockchainLogType DetectType()
    {
        var signature = Topics[0].ToString();

        return signature switch
        {
            UniswapWellKnownField.V3CollectSignature => BlockchainLogType.Collect,
            UniswapWellKnownField.V4ModifyLiquiditySignature => BlockchainLogType.ModifyLiquidity
        };
    }
}

public enum BlockchainLogType
{
    IncreaseLiquidity,
    DecreaseLiquidity,
    Collect,
    ModifyLiquidity,
    Mint
}