using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Aave.Application.Abstractions;

/// <summary>
/// Provides functionality to synchronize Aave lending positions for cryptocurrency wallets.
/// This service is responsible for fetching, updating, and persisting Aave positions data
/// in the context of a given wallet.
/// </summary>
public interface IAavePositionsSyncService
{
    /// <summary>
    /// Synchronizes the Aave lending positions for a given wallet. This method fetches
    /// the current lending positions from all supported Aave networks, processes the data,
    /// and updates the repository storage accordingly.
    /// </summary>
    /// <param name="protocol"></param>
    /// <param name="wallet">The wallet entity containing the address to fetch lending positions for.</param>
    /// <param name="syncDay"></param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<List<AavePosition>> SyncPositionsAsync(AaveProtocolConfiguration protocol, Wallet wallet, DateOnly syncDay,
        CancellationToken ct = default);
}