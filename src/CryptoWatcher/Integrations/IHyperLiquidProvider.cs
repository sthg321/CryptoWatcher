using CryptoWatcher.Entities;

namespace CryptoWatcher.Abstractions.Integrations;

public interface IHyperLiquidProvider
{
    Task<decimal> GetVaultsBalance(Wallet wallet, CancellationToken ct = default);
}