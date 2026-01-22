using System.Numerics;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.UniswapV3.OperationReaders;

public class DecreaseLiquidityOperationApplier : BasePositionOperationApplier<DecreaseLiquidityOperation>
{
    public DecreaseLiquidityOperationApplier(ITokenEnricher tokenEnricher) : base(tokenEnricher)
    {
    }

    protected override async ValueTask ApplyOperation(UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        TokenInfoPair enrichedTokens, DecreaseLiquidityOperation operation, DateTime timestamp)
    {
        position.AddCashFlow(CashFlowEvent.Withdrawal, enrichedTokens, operation.TransactionHash, timestamp);

        if (operation.Commission0 != 0 || operation.Commission1 != 0)
        {
            var enrichedCommission = await EnrichTokensAsync(chainConfiguration,
                CreateTokenFromPosition(operation.Commission0, position.Token0),
                CreateTokenFromPosition(operation.Commission1, position.Token1));

            position.AddCashFlow(CashFlowEvent.FeeClaim, enrichedCommission, operation.TransactionHash, timestamp);
        }

        if (position.IsClosed)
        {
            position.ClosePosition(DateOnly.FromDateTime(timestamp));
        }
    }

    private static Token CreateTokenFromPosition(BigInteger amount, CryptoToken cryptoToken) => new()
    {
        Address = cryptoToken.Address,
        Balance = amount
    };
}