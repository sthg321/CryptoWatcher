using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsSnapshotSynchronization.Models;

public class PositionValuation
{
    public required TokenInfoPair PositionTokens { get; init; }

    public required TokenInfoPair PositionFees { get; init; }

    public required bool IsInRange { get; init; }
}