namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapWalletSyncOrchestrator
{
    Task SyncWalletAsync( CancellationToken ct = default);
}