using CryptoWatcher.Entities;

namespace CryptoWatcher.Integrations;

public interface IHyperLiquidProvider
{
    Task<decimal> GetVaultsBalance(Wallet wallet, CancellationToken ct = default);
}