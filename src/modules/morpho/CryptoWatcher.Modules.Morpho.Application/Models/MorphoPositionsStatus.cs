using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Application.Models;

public class MorphoPositionsStatus
{
    public double HealthFactor { get; set; }

    public decimal CollateralLiquidationPrice { get; set; }
    
    public CryptoToken Collateral { get; set; } = null!;

    public CryptoToken Load { get; set; } = null!;
}