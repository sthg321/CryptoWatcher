using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Models;

public abstract class PlatformDailyReport
{
    public required Money PositionInUsd { get; init; }

    public required Money ProfitInUsd { get; init; }

    public required Percent ProfitInPercent { get; init; }
}