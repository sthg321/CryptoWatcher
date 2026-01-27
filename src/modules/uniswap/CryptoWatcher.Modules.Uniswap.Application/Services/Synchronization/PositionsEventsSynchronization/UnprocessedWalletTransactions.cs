using System.Runtime.CompilerServices;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization;

public class UnprocessedWalletTransactions : IUnprocessedWalletTransactions
{
    private readonly IWalletTransactionPaginator _walletTransactionPaginator;

    public UnprocessedWalletTransactions(IWalletTransactionPaginator walletTransactionPaginator)
    {
        _walletTransactionPaginator = walletTransactionPaginator;
    }
    
    public async IAsyncEnumerable<BlockchainTransaction> GetTransactionsAsync(
        UniswapChainConfiguration chainConfiguration,
        UniswapSynchronizationState synchronizationState,
        EvmAddress walletAddress,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var walletTransactions in _walletTransactionPaginator.PaginateWalletTransactionsAsync(
                           chainConfiguration,
                           walletAddress,
                           ct))
        {
            if (walletTransactions.Count == 0)
            {
                yield break;
            }

            foreach (var walletTransaction in walletTransactions)
            {
                // for case when transaction removed from block so we still know what was processed
                if (walletTransaction.BlockNumber < synchronizationState.LastBlockNumber)
                {
                    yield break;
                }
                
                if (walletTransaction.BlockNumber == synchronizationState.LastBlockNumber &&
                    walletTransaction.Hash.Equals(synchronizationState.LastTransactionHash))
                {
                    yield break;
                }

                yield return walletTransaction;
            }
        }
    }
}