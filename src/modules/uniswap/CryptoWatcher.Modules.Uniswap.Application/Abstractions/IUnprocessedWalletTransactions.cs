using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUnprocessedWalletTransactions
{
    IAsyncEnumerable<BlockchainTransaction> GetTransactionsAsync(
        UniswapChainConfiguration chainConfiguration,
        UniswapSynchronizationState synchronizationState,
        EvmAddress walletAddress,
        CancellationToken ct = default);
}