namespace CryptoWatcher.HyperliquidModule.Models;

public record HyperliquidVaultReportItem
{
    public required string VaultAddress { get; init; } = null!;

    public required DateOnly Day { get; init; }

    public required decimal Balance { get; init; }

    public required decimal DailyProfit { get; init; }

    public required decimal DailyPercentProfit { get; init; }
}