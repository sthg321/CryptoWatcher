using CryptoWatcher.Modules.Hyperliquid.Application.Models;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;

public interface IHyperliquidProvider
{
    Task<HyperliquidPositionCashFlow[]> GetCashFlowEventsAsync(Wallet wallet,
        DateOnly from, DateOnly to,
        CancellationToken ct = default);

    Task<IReadOnlyCollection<HyperliquidVault>> GetVaultsPositionsEquityAsync(Wallet wallet,
        CancellationToken ct = default);
}