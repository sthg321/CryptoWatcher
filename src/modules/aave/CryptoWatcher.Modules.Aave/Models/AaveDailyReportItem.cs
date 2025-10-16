using CryptoWatcher.Models;

namespace CryptoWatcher.Modules.Aave.Models;

public class AaveDailyReportItem : PlatformDailyReportItem
{
    public string TokenSymbol { get; init; } = null!;
 
    public required decimal PositionGrowInUsd { get; init; }
    
    public required decimal PositionInToken { get; init; }
    
    public required decimal DailyProfitInToken { get; init; }
  
}