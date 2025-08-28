namespace CryptoWatcher.Entities.Aave;

public class LendingPositionHistory
{
    public string Network { get; set; } = null!;

    public DateOnly Day { get; set; }
    
    public LendingPosition LendingPosition { get; set; } = null!;
}