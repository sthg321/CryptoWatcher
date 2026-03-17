using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.WalletIngestion.Application.Models;
using CryptoWatcher.Modules.WalletIngestion.Entities;

namespace CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;

public interface IUnprocessedWalletTransactions
{
    IAsyncEnumerable<BlockchainTransaction> GetTransactionsAsync(WalletIngestionCheckpoint ingestionCheckpoint,
        CancellationToken ct = default);
}