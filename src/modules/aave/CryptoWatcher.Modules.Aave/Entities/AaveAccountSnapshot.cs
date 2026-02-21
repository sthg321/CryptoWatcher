using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Entities;

public class AaveAccountSnapshot
{
    public double HealthFactor { get; init; }

    public DateOnly Day { get; init; }

    public EvmAddress WalletAddress { get; init; } = null!;

    public string NetworkName { get; init; } = null!;

    public decimal TotalCollateralInUsd { get; init; }

    public decimal TotalDebtInUsd { get; init; }
}