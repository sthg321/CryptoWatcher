using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.EventAppliers;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.PositionEventAppliers;

public abstract class BasePositionEventApplier<TOperation> : IPositionMutationEvent
    where TOperation : PositionEvent
{
    private readonly ITokenEnricher _tokenEnricher;

    protected BasePositionEventApplier(ITokenEnricher tokenEnricher)
    {
        _tokenEnricher = tokenEnricher;
    }

    public async Task<UniswapLiquidityPosition> ApplyOperationAsync(
        UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        PositionEvent @event,
        DateTime timestamp,
        CancellationToken ct = default)
    {
        var enrichedTokens = await EnrichTokensAsync(chainConfiguration, @event.Token0, @event.Token1, ct);

        await ApplyOperation(chainConfiguration, position, enrichedTokens, (TOperation)@event, timestamp);

        return position;
    }
    
    protected async Task<TokenInfoPair> EnrichTokensAsync(UniswapChainConfiguration chainConfiguration, Token token0,
        Token token1, CancellationToken ct = default)
    {
        return await _tokenEnricher.EnrichAsync(chainConfiguration.Name,
            chainConfiguration.RpcUrlWithAuthToken,
            new TokenPair
            {
                Token0 = token0,
                Token1 = token1
            }, ct);
    }
    
    protected abstract ValueTask ApplyOperation(
        UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        TokenInfoPair enrichedTokens,
        TOperation operation,
        DateTime timestamp);
}