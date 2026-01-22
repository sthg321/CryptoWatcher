using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;

public interface IPositionOperationApplierFactory
{
    IPositionMutationOperation GetOperationApplier(PositionOperation operation);
}