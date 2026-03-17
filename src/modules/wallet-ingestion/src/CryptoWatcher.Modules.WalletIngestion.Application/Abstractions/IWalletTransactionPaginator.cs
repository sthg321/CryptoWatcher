using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.WalletIngestion.Application.Models;
using CryptoWatcher.Modules.WalletIngestion.Entities;

namespace CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;

public interface IWalletTransactionPaginator
{
    IAsyncEnumerable<IReadOnlyCollection<BlockchainTransaction>> PaginateWalletTransactionsAsync(
        WalletIngestionCheckpoint checkpoint,
        CancellationToken ct = default);
}