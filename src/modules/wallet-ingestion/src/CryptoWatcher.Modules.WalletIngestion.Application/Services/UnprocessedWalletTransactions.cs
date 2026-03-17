using System.Runtime.CompilerServices;
using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;
using CryptoWatcher.Modules.WalletIngestion.Application.Models;
using CryptoWatcher.Modules.WalletIngestion.Entities;

namespace CryptoWatcher.Modules.WalletIngestion.Application.Services;

public class UnprocessedWalletTransactions : IUnprocessedWalletTransactions
{
    private readonly IWalletTransactionPaginator _walletTransactionPaginator;

    public UnprocessedWalletTransactions(IWalletTransactionPaginator walletTransactionPaginator)
    {
        _walletTransactionPaginator = walletTransactionPaginator;
    }

    public async IAsyncEnumerable<BlockchainTransaction> GetTransactionsAsync(
        WalletIngestionCheckpoint ingestionCheckpoint,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var walletTransactions in _walletTransactionPaginator.PaginateWalletTransactionsAsync(
                           ingestionCheckpoint, ct))
        {
            if (walletTransactions.Count == 0)
            {
                yield break;
            }

            foreach (var walletTransaction in walletTransactions)
            {
                // for case when transaction removed from block so we still know what was processed
                if (walletTransaction.BlockNumber < ingestionCheckpoint.LastPublishedBlockNumber)
                {
                    yield break;
                }

                if (walletTransaction.BlockNumber == ingestionCheckpoint.LastPublishedBlockNumber &&
                    walletTransaction.Hash.Equals(ingestionCheckpoint.LastPublishedTransactionHash))
                {
                    yield break;
                }

                yield return walletTransaction;
            }
        }
    }
}