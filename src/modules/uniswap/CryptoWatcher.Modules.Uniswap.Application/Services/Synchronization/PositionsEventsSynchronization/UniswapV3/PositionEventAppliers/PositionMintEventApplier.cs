using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.EventAppliers;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models
    .PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    PositionEventAppliers;

public class PositionMintEventApplier : IPositionMintEventApplier
{
    private readonly ITokenEnricher _tokenEnricher;

    public PositionMintEventApplier(ITokenEnricher tokenEnricher)
    {
        _tokenEnricher = tokenEnricher;
    }

    public async Task<UniswapLiquidityPosition> CreatePositionAsync(
        MintPositionEvent @event,
        UniswapChainConfiguration chainConfiguration,
        DateTime timestamp,
        CancellationToken ct = default)
    {
        var enrichedTokens = await _tokenEnricher.EnrichAsync(chainConfiguration.Name,
            chainConfiguration.RpcUrlWithAuthToken,
            new TokenPair
            {
                Token0 = @event.Token0,
                Token1 = @event.Token1
            }, ct);

        var position = new UniswapLiquidityPosition(@event.PositionId,
            @event.TickLower,
            @event.TickUpper,
            enrichedTokens.Token0,
            enrichedTokens.Token1,
            @event.From,
            chainConfiguration,
            DateOnly.FromDateTime(timestamp));

        return position;
    }
}