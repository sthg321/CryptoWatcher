namespace CryptoWatcher.UniswapModule.Models;

public class PositionInPool
{
    public required ulong PositionId { get; set; }
    
    public required bool IsInRange { get; init; }
    
    public required TokenPair TokenInfoPair { get; init; } = null!;
}