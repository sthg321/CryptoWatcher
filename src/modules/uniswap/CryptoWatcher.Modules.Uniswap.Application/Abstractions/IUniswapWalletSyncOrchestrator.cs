namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapWalletSyncOrchestrator
{
    Task SyncWalletPositionsAsync( CancellationToken ct = default);
}