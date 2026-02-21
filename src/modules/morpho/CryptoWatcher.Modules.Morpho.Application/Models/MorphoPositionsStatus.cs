using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Application.Models;

public class MorphoPositionsStatus
{
    public EvmAddress Wallet { get; set; } = null!;

    public string Network { get; set; } = null!;
    
    public double HealthFactor { get; set; }

    public decimal CollateralLiquidationPrice { get; set; }
    
    public CryptoToken Collateral { get; set; } = null!;

    public CryptoToken Load { get; set; } = null!;
}