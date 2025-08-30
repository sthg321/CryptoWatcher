namespace CryptoWatcher.HyperliquidModule.Models;

public record HyperliquidVaultReportItem
{
    public required string VaultAddress { get; init; } = null!;

    public required DateOnly Day { get; init; }

    public required decimal Balance { get; init; }

    public required decimal DailyChange { get; init; }

    public required decimal DailyChangePercent { get; init; }
}