using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.UniswapV3.OperationReaders;

public class CollectFeesOperationApplier : BasePositionOperationApplier<CollectFeesOperation>
{
    public CollectFeesOperationApplier(ITokenEnricher tokenEnricher) : base(tokenEnricher)
    {
    }

    protected override ValueTask ApplyOperation(UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        TokenInfoPair enrichedTokens,
        CollectFeesOperation operation,
        DateTime timestamp)
    {
        position.AddCashFlow(CashFlowEvent.FeeClaim, enrichedTokens, operation.TransactionHash, timestamp);

        return ValueTask.CompletedTask;
    }
}