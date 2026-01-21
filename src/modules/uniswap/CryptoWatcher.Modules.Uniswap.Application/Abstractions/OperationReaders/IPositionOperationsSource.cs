using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;

public interface IPositionOperationsSource
{
    Task<PositionOperationInfo?> GetOperationFromTransactionAsync(UniswapChainConfiguration chainConfiguration,
        TransactionHash transactionHash,
        CancellationToken ct = default);
}