using CryptoWatcher.Modules.Hyperliquid.Application.Models;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;

public interface IHyperliquidGateway
{
    Task<HyperliquidPositionCashFlow[]> GetCashFlowEventsAsync(Wallet wallet,
        DateTime from, DateTime to,
        CancellationToken ct = default);

    Task<IReadOnlyCollection<HyperliquidVault>> GetVaultsPositionsEquityAsync(Wallet wallet,
        CancellationToken ct = default);
}