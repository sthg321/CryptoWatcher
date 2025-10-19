using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IBlockscoutTransactionFetcher
{
    IAsyncEnumerable<BlockscoutTransaction> GetTransactionsAsync(UniswapChainConfiguration chain,
        Wallet wallet,
        TransactionHash? stopAtTransactionHash,
        CancellationToken ct = default);
}