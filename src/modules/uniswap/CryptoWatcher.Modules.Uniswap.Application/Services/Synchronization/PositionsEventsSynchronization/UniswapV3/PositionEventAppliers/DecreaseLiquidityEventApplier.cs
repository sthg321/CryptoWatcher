using System.Numerics;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    PositionEventAppliers;

public class DecreaseLiquidityEventApplier : BasePositionEventApplier<DecreaseLiquidityEvent>
{
    public DecreaseLiquidityEventApplier(ITokenEnricher tokenEnricher) : base(tokenEnricher)
    {
    }

    protected override async ValueTask ApplyOperation(UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        TokenInfoPair enrichedTokens, DecreaseLiquidityEvent @event, DateTime timestamp)
    {
        if (@event.Commission0 != 0 || @event.Commission1 != 0)
        {
            var enrichedCommission = new TokenInfoPair
            {
                Token0 = await EnrichTokenAsync(chainConfiguration,
                    CreateTokenFromPosition(@event.Commission0, position.Token0)),
                Token1 = await EnrichTokenAsync(chainConfiguration,
                    CreateTokenFromPosition(@event.Commission1, position.Token1)),
            };

            position.AddCashFlow(CashFlowEvent.FeeClaim, enrichedCommission, @event.TransactionHash, timestamp);
        }

        position.AddCashFlow(CashFlowEvent.Withdrawal, enrichedTokens, @event.TransactionHash, timestamp);

        if (@event.IsPositionClosed)
        {
            position.ClosePosition(DateOnly.FromDateTime(timestamp));
        }
    }

    protected override async Task<TokenInfoPair> EnrichTokensAsync(UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        DecreaseLiquidityEvent @event, CancellationToken ct = default)
    {
        if (!@event.IsPositionClosedWithOneTokenOnly())
        {
            return await base.EnrichTokensAsync(chainConfiguration, position, @event, ct);
        }

        if (@event.Token0 is null && @event.Token1 is null)
        {
            throw new InvalidOperationException("At least one token must exist in position");
        }
        
        // Тут мы попадаем в случай, когда пул из 2 токенов перелился в 1 токен.
        // В таком случае в событии есть информация только о том токене, который != 0
        var (presentEventToken, missingPositionToken) = @event.Token0 is not null
            ? (@event.Token0, position.Token1)
            : (@event.Token1!, position.Token0);

        var enrichedPresent = await EnrichTokenAsync(chainConfiguration, presentEventToken, ct);
        var enrichedMissing = await EnrichTokenAsync(chainConfiguration, new Token
        {
            Address = missingPositionToken.Address,
            Balance = BigInteger.Zero
        }, ct);

        // Нет гарантии, что token0 будет такой же, как и в позиции, потому что мы не знаем, в какой токен перелился пул.
        // Восстанавливаем порядок токенов как в позиции
        var isToken0Present = enrichedPresent.Address.Equals(position.Token0.Address);

        return isToken0Present
            ? new TokenInfoPair { Token0 = enrichedPresent, Token1 = enrichedMissing }
            : new TokenInfoPair { Token0 = enrichedMissing, Token1 = enrichedPresent };
    }

    private static Token CreateTokenFromPosition(BigInteger amount, CryptoToken cryptoToken) => new()
    {
        Address = cryptoToken.Address,
        Balance = amount
    };
}