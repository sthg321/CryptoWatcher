using CryptoWatcher.Modules.Morpho.Abstractions;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Entities;

public class MorphoMarketPositionSnapshot : IMarketPositionSnapshot
{
    //for ef
    private MorphoMarketPositionSnapshot()
    {
        
    }
    
    public MorphoMarketPositionSnapshot(
        Guid morphoMarketPositionId,
        DateOnly day, 
        CryptoTokenStatistic loadToken,
        CryptoTokenStatistic collateralToken,
        double healthFactor,
        double liquidationLtv)
    {
        MorphoMarketPositionId = morphoMarketPositionId;
        Day = day;
        LoadToken = loadToken;
        CollateralToken = collateralToken;
        HealthFactor = healthFactor;
        LiquidationLtv = liquidationLtv;
    }

    public DateOnly Day { get; private set; }

    public double HealthFactor { get; private set; }

    public double LiquidationLtv { get; private set; }

    public CryptoTokenStatistic LoadToken { get; private set; } = null!;

    public CryptoTokenStatistic CollateralToken { get; private set; } = null!;

    public Guid MorphoMarketPositionId { get; private set; }

    public void UpdateSnapshot(CryptoTokenStatistic loadToken, CryptoTokenStatistic collateralToken,
        double healthFactor, double liquidationLtv)
    {
        LoadToken = loadToken;
        CollateralToken = collateralToken;
        HealthFactor = healthFactor;
        LiquidationLtv = liquidationLtv;
    }

    public decimal CalculateCollateralPriceForLiquidation()
    {
        if (CollateralToken.AmountInUsd == 0)
        {
            return 0;
        }

        return LoadToken.AmountInUsd / (CollateralToken.AmountInUsd * (decimal)LiquidationLtv);
    }
}