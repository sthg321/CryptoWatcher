namespace CryptoWatcher.Modules.Aave.Application.Models;

public record AavePositionsResponse(IReadOnlyCollection<AaveLendingPosition> Positions, double HealthFactor);