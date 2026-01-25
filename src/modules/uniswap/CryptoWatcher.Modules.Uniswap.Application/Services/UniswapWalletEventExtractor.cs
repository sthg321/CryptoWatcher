using System.Runtime.CompilerServices;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapWalletEventExtractor : IUniswapWalletEventExtractor
{
    private readonly IUnprocessedWalletTransactions _unprocessedWalletTransactions;
    private readonly IUniswapTransactionEnricher _uniswapTransactionEnricher;

    public UniswapWalletEventExtractor(IUnprocessedWalletTransactions unprocessedWalletTransactions,
        IUniswapTransactionEnricher uniswapTransactionEnricher)
    {
        _unprocessedWalletTransactions = unprocessedWalletTransactions;
        _uniswapTransactionEnricher = uniswapTransactionEnricher;
    }

    public async IAsyncEnumerable<UniswapEvent> ExtractUniswapEventsAsync(UniswapChainConfiguration chain,
        UniswapSynchronizationState synchronizationState, EvmAddress wallet,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var transaction in _unprocessedWalletTransactions.GetTransactionsAsync(chain,
                           synchronizationState, wallet, ct))
        {
            var operation = await _uniswapTransactionEnricher.TryEnrichAsync(chain, transaction, ct);

            if (operation is null)
            {
                continue;
            }

            yield return operation;
        }
    }
}