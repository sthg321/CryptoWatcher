using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;

public interface IPositionOperationApplierFactory
{
    IPositionOperationApplier<TPositionOperation> GetOperationApplier<TPositionOperation>()
        where TPositionOperation : PositionOperation;
}