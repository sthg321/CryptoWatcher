using System.Runtime.CompilerServices;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class BlockscoutTransactionFetcher : IBlockscoutTransactionFetcher
{
    private readonly IBlockscoutProvider _blockscoutProvider;
    private readonly ILogger<BlockscoutTransactionFetcher> _logger;

    public BlockscoutTransactionFetcher(IBlockscoutProvider blockscoutProvider,
        ILogger<BlockscoutTransactionFetcher> logger)
    {
        _blockscoutProvider = blockscoutProvider;
        _logger = logger;
    }

    public async IAsyncEnumerable<BlockscoutTransaction> GetTransactionsAsync(UniswapChainConfiguration chain,
        Wallet wallet,
        TransactionHash? stopAtTransactionHash,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        BlockscoutNextPageParams? nextParams = null;
        while (true)
        {
            _logger.LogInformation("Fetching transactions for uniswap chain {ChainName}", chain.Name);
            
            var page = await _blockscoutProvider.GetAccountTransactionsAsync(chain, wallet.Address, nextParams, ct);
            if (page.Transactions.Count == 0)
            {
                _logger.LogInformation("No transactions found for uniswap chain {ChainName}", chain.Name);
                yield break;
            }

            // all transactions from block scout are sorted in desc order
            var latestTransaction = page.Transactions.First();
            if (latestTransaction.TransactionHash.Equals(stopAtTransactionHash))
            {
                // there is no new transactions
                _logger.LogInformation("There is no any new transactions. Last  transaction is: {LastTransaction}",
                    latestTransaction.TransactionHash);
                yield break;
            }

            _logger.LogInformation("Found {NewTransactionCount} new transactions for uniswap chain {ChainName}",
                page.Transactions.Count, chain.Name);

            foreach (var transaction in page.Transactions)
            {
                if (transaction.TransactionHash == stopAtTransactionHash)
                {
                    _logger.LogInformation(
                        "Stop processing transactions. Fond last processed transaction: {LastTransaction}",
                        stopAtTransactionHash.Value);
                    
                    yield break;
                }

                yield return transaction;
            }

            if (page.NextPageParams is not null)
            {
                _logger.LogInformation("Start processing next page.");
                nextParams = page.NextPageParams;
                continue;
            }

            break;
        }
    }
}