using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Models;

public class ProfitMetric
{
    public required Money Amount { get; init; }

    public required Percent Percent { get; init; }

    public static ProfitMetric Empty() => new() { Amount = 0, Percent = 0 };
}