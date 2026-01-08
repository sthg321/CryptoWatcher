namespace CryptoWatcher.Modules.Merkl.ValueObjects;

public record RewardStatus
{
    public decimal ClaimedAmount { get; set; }

    public decimal ClaimabelAmount { get; set; }

    public decimal PendingAmount { get; set; }
}