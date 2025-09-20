using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Models;

public class PlatformDailyReportData
{
    public required string PlatformName { get; set; } = null!;
    
    public required Dictionary<Wallet, List<PlatformDailyReport>> Reports { get; set; } = [];
}