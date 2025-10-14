using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface ITransactionDataProvider
{
    Task<TransactionData?> GetTransactionDataAsync(UniswapChainConfiguration chain,
        TransactionHash transactionHash, CancellationToken ct = default);
}