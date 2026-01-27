using System.Runtime.CompilerServices;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization;

public class UniswapWalletTransactionScanner : IUniswapWalletTransactionScanner
{
    private readonly IUnprocessedWalletTransactions _unprocessedWalletTransactions;
    private readonly IUniswapTransactionEnricher _uniswapTransactionEnricher;

    public UniswapWalletTransactionScanner(IUnprocessedWalletTransactions unprocessedWalletTransactions,
        IUniswapTransactionEnricher uniswapTransactionEnricher)
    {
        _unprocessedWalletTransactions = unprocessedWalletTransactions;
        _uniswapTransactionEnricher = uniswapTransactionEnricher;
    }

    public async IAsyncEnumerable<WalletTransactionScanResult> ScanWalletTransactionsAsync(
        UniswapChainConfiguration chain,
        UniswapSynchronizationState synchronizationState, EvmAddress wallet,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var transaction in _unprocessedWalletTransactions.GetTransactionsAsync(chain,
                           synchronizationState, wallet, ct))
        {
            var uniswapEvent = await _uniswapTransactionEnricher.TryEnrichAsync(chain, transaction, ct);

            yield return new WalletTransactionScanResult
            {
                Event = uniswapEvent,
                Transaction = transaction
            };
        }
    }
}