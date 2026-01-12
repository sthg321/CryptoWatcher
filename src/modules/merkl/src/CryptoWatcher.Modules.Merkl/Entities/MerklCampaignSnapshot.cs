using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.Modules.Merkl.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Entities;

public class MerklCampaignSnapshot : IPositionSnapshot
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

    public decimal ClaimableAmount { get; private set; }

    public decimal ClaimedAmount { get; private set; }

    public decimal PendingAmount { get; private set; }

    public decimal PriceInUsd { get; private set; }

    public decimal RewardsAmount { get; private set; }
    
    public Guid MerklCampaignId { get; private init; }

    public void Update(RewardStatus rewardStatus, decimal priceInUsd)
    {
        UpdateFromRewards(rewardStatus);
        PriceInUsd = priceInUsd;
    }

    private void UpdateFromRewards(RewardStatus rewardStatus)
    {
        // no changes in rewards
        if (ClaimableAmount == rewardStatus.ClaimabelAmount &&
            PendingAmount == rewardStatus.PendingAmount &&
            ClaimedAmount == rewardStatus.ClaimedAmount)
        {
            return;
        }
        
        RewardsAmount += rewardStatus.ClaimabelAmount - rewardStatus.ClaimedAmount + rewardStatus.PendingAmount;
        ClaimableAmount = rewardStatus.ClaimabelAmount;
        PendingAmount = rewardStatus.PendingAmount;
        ClaimedAmount = rewardStatus.ClaimedAmount;
    }
}