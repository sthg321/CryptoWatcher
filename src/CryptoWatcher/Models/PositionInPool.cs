namespace CryptoWatcher.Models;

public class PositionInPool
{
    public required ulong PositionId { get; set; }
    
    public required bool IsInRange { get; set; }
    
    public required TokenPair TokenInfoPair { get; init; } = null!;
}