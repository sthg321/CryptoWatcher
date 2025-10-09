using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface ITransactionDataProvider
{
    Task<TransactionData?> GetTransactionDataAsync(UniswapChainConfiguration chain,
        string transactionHash, CancellationToken ct = default);
}