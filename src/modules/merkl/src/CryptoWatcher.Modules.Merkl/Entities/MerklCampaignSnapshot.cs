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

    public decimal ClaimabelAmount { get; private set; }

    public decimal ClaimedAmount { get; private set; }

    public decimal PendingAmout { get; private set; }

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
        if (ClaimabelAmount == rewardStatus.ClaimabelAmount &&
            PendingAmout == rewardStatus.PendingAmount &&
            ClaimedAmount == rewardStatus.ClaimedAmount)
        {
            return;
        }
        
        // user claimed only avilabel rewarads and still has pending rewards
        if (ClaimabelAmount == rewardStatus.ClaimabelAmount &&
            PendingAmout == rewardStatus.PendingAmount &&
            ClaimedAmount != rewardStatus.ClaimedAmount)
        {
            ClaimabelAmount = rewardStatus.ClaimabelAmount;
            return;
        }
        
        //user has only avilable rewards without any pending rewards
        
        RewardsAmount += rewardStatus.ClaimabelAmount - rewardStatus.ClaimedAmount + rewardStatus.PendingAmount;
        ClaimabelAmount = rewardStatus.ClaimabelAmount;
        PendingAmout = rewardStatus.PendingAmount;
        ClaimedAmount = rewardStatus.ClaimedAmount;
    }
}