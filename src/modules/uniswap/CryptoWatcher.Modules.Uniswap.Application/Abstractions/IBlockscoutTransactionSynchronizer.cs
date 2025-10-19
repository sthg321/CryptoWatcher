using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IBlockscoutTransactionSynchronizer
{
    Task SyncAsync(UniswapChainConfiguration chain, Wallet wallet, CancellationToken ct = default);
}