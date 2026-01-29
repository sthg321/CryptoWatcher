using CryptoWatcher.Modules.Uniswap.Application.Abstractions.EventAppliers;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.PositionEventAppliers;

public class PositionEventApplierFactory : IPositionEventApplierFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PositionEventApplierFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPositionMutationEvent GetEventApplier(PositionEvent @event)
    {
        return _serviceProvider.GetRequiredKeyedService<IPositionMutationEvent>(@event.GetType());
    }
}