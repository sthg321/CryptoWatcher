using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.EventAppliers;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models
    .PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    PositionEventAppliers;

public abstract class BasePositionEventApplier<TOperation> : IPositionMutationEvent
    where TOperation : PositionEvent
{
    private readonly ITokenEnricher _tokenEnricher;

    protected BasePositionEventApplier(ITokenEnricher tokenEnricher)
    {
        _tokenEnricher = tokenEnricher;
    }

    public async Task<UniswapLiquidityPosition> ApplyOperationAsync(UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        PositionEvent @event, DateTime timestamp, CancellationToken ct = default)
    {
        return await ApplyOperationAsync(chainConfiguration, position, (TOperation)@event, timestamp, ct);
    }

    protected virtual async Task<TokenInfoPair> EnrichTokensAsync(UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        TOperation @event,
        CancellationToken ct = default)
    {
        return await _tokenEnricher.EnrichAsync(chainConfiguration.Name,
            chainConfiguration.RpcUrlWithAuthToken,
            new TokenPair
            {
                Token0 = @event.Token0,
                Token1 = @event.Token1
            }, ct);
    }

    protected async Task<CryptoToken> EnrichTokenAsync(UniswapChainConfiguration chainConfiguration, Token token0,
        CancellationToken ct = default)
    {
        return await _tokenEnricher.EnrichAsync(chainConfiguration.Name,
            chainConfiguration.RpcUrlWithAuthToken, token0, ct);
    }

    protected abstract ValueTask ApplyOperation(
        UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        TokenInfoPair enrichedTokens,
        TOperation operation,
        DateTime timestamp);

    private async Task<UniswapLiquidityPosition> ApplyOperationAsync(
        UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        TOperation @event,
        DateTime timestamp,
        CancellationToken ct = default)
    {
        var enrichedTokens = await EnrichTokensAsync(chainConfiguration, position, @event, ct);

        await ApplyOperation(chainConfiguration, position, enrichedTokens, @event, timestamp);

        return position;
    }
}