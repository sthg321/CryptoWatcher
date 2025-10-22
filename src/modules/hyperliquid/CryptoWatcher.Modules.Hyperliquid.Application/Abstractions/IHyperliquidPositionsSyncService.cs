using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;

public interface IHyperliquidPositionsSyncService
{
    /// <summary>
    /// Synchronizes the positions of the given wallet for the specified day.
    /// </summary>
    /// <param name="wallet">The cryptocurrency wallet whose positions are to be synchronized.</param>
    /// <param name="from">The specific day for which positions are being synchronized.</param>
    /// <param name="to"></param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous synchronization operation.</returns>
    Task SyncPositionsAsync(Wallet wallet, DateOnly from, DateOnly to, CancellationToken ct = default);
}