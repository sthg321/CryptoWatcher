using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Application.Models;

public record MorphoMarketPositionData(
    Guid MarketId,
    CryptoToken LoanToken,
    CryptoToken CollateralToken,
    double HealthFactor)
{
    /// <summary>
    /// if a position has been created at least once, the API will always return it.
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() => LoanToken.Amount == 0 && CollateralToken.Amount == 0;
}