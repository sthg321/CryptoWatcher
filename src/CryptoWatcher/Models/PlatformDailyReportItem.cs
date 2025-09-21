using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Models;

public class PlatformDailyReportItem
{
    public required DateOnly Day { get; init; }

    public required Money PositionInUsd { get; init; }
    
    public required Money DailyProfitInUsd { get; init; }
  
    public required Percent DailyProfitInUsdPercent { get; init; }
}