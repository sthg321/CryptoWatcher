using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IWalletTransactionPaginator
{
    IAsyncEnumerable<IReadOnlyCollection<BlockchainTransaction>> PaginateWalletTransactionsAsync(
        UniswapChainConfiguration chainConfiguration,
        EvmAddress walletAddress,
        CancellationToken ct = default);
}