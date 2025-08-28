using CryptoWatcher.Entities;
using CryptoWatcher.Entities.Hyperliquid;

namespace CryptoWatcher.Integrations;

public interface IHyperliquidProvider
{
    Task<HyperliquidVaultEvent[]> GetVaultsFundingHistory(Wallet wallet,
        CancellationToken ct = default);
    
    Task<(string VaultAddress, decimal Equity)[]> GetVaultsPositionsEquityAsync(Wallet wallet,
        CancellationToken ct = default);
}