using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Models;

public class PlatformDailyReportData
{
    public required string PlatformName { get; init; } = null!;
    
    public required Dictionary<Wallet, List<PlatformDailyReport>> Reports { get; init; } = [];
}