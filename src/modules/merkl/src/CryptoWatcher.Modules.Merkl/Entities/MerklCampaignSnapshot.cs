using CryptoWatcher.Modules.Merkl.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Entities;

public class MerklCampaignSnapshot
{
    private MerklCampaignSnapshot()
    {
    }

    public MerklCampaignSnapshot(DateOnly day, RewardStatus rewardStatus,
        decimal priceInUsd,
        Guid merklCampaignId)
    {
        Day = day;
        PriceInUsd = priceInUsd;
        MerklCampaignId = merklCampaignId;
        UpdateFromRewards(rewardStatus);
    }

    public DateOnly Day { get; private init; }
    
    public decimal ClaimabelAmount { get; private set; }

    public decimal ClaimedAmount { get; private set; }

    public decimal PendingAmout { get; private set; }

    public decimal PriceInUsd { get; private set; }

    public Guid MerklCampaignId { get; private init; }
    
    public decimal NetAmount => ClaimabelAmount - ClaimedAmount + PendingAmout;

    public void Update(RewardStatus rewardStatus, decimal priceInUsd)
    {
        UpdateFromRewards(rewardStatus);
        PriceInUsd = priceInUsd;
    }

    private void UpdateFromRewards(RewardStatus rewardStatus)
    {
        ClaimabelAmount = rewardStatus.ClaimabelAmount;
        ClaimedAmount = rewardStatus.ClaimedAmount;
        PendingAmout = rewardStatus.PendingAmount;
    }
}