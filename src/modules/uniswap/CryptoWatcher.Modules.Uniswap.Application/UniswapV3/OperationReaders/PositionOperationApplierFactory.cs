using CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Modules.Uniswap.Application.UniswapV3.OperationReaders;

public class PositionOperationApplierFactory : IPositionOperationApplierFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PositionOperationApplierFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPositionOperationApplier<TPositionOperation> GetOperationApplier<TPositionOperation>()
        where TPositionOperation : PositionOperation
    {
        return _serviceProvider.GetRequiredKeyedService<IPositionOperationApplier<TPositionOperation>>(
            typeof(TPositionOperation));
    }
}