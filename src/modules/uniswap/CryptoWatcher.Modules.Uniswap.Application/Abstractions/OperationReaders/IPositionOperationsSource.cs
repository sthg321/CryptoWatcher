using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;

public interface IPositionOperationsSource
{
    Task<PositionOperation?> GetOperationFromTransactionAsync(UniswapChainConfiguration chainConfiguration,
        TransactionHash hash,
        CancellationToken ct = default);
}