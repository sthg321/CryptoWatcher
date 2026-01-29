using CryptoWatcher.Exceptions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.EventAppliers;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.PositionEventAppliers;

public class UniswapV3PositionEventApplier : IUniswapPositionEventApplier
{
    private readonly IPositionEventApplierFactory _eventApplierFactory;
    private readonly IPositionMintEventApplier _positionMintEventApplier;

    public UniswapV3PositionEventApplier(IPositionEventApplierFactory eventApplierFactory,
        IPositionMintEventApplier positionMintEventApplier)
    {
        _eventApplierFactory = eventApplierFactory;
        _positionMintEventApplier = positionMintEventApplier;
    }

    public async Task<UniswapLiquidityPosition> ApplyOperationToPositionAsync(
        UniswapChainConfiguration chainConfiguration,
        EvmAddress walletAddress,
        UniswapPositionEvent positionEvent,
        UniswapLiquidityPosition? position, 
        CancellationToken ct = default)
    {
        if (positionEvent.Event is MintPositionEvent mintPositionOperation)
        {
            return await _positionMintEventApplier.CreatePositionAsync(walletAddress, mintPositionOperation,
                chainConfiguration, positionEvent.Timestamp, ct);
        }

        if (position is null)
        {
            throw new DomainException("Position is not found for event");
        }

        if (position.IsClosed)
        {
            return position;
        }
        
        var applier = _eventApplierFactory.GetEventApplier(positionEvent.Event);

        return await applier.ApplyOperationAsync(chainConfiguration, position!, positionEvent.Event,
            positionEvent.Timestamp, ct);
    }
}