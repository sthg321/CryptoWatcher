using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions.EventAppliers;

public interface IPositionEventApplierFactory
{
    IPositionMutationEvent GetEventApplier(PositionEvent @event);
}