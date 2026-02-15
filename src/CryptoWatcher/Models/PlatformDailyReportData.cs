using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Models;

public class PlatformDailyReportData
{
    public required string PlatformName { get; init; } = null!;
    
    public required Dictionary<EvmAddress, List<PlatformDailyReport>> Reports { get; init; } = [];
}

