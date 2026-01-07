using CryptoWatcher.Modules.Merkl.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Entities;

public class MerklCampaign
{
    private readonly List<MerklCampaignSnapshot> _snapshots = [];

    private MerklCampaign()
    {
        
    }
    
    public MerklCampaign(int chainId, TransactionHash campaignId, EvmAddress walletAddress, Asset asset)
    {
        ChainId = chainId;
        CampaignId = campaignId;
        WalletAddress = walletAddress;
        Asset = asset;
    }
    
    public int ChainId { get; init; }
    
    /// <summary>
    /// Campaign identifier on Merkl
    /// </summary>
    public TransactionHash CampaignId { get; private set; }  
    
    public EvmAddress WalletAddress { get; private set; } 
    
    public Asset Asset { get; private set; }  

    public IReadOnlyCollection<MerklCampaignSnapshot> Snapshots => _snapshots;

    public void AddOrdUpdateSnapshot(DateOnly day, decimal claimedInUsd, decimal pendingInUsd, string reason)
    {
        var existedSnapshot = _snapshots.FirstOrDefault(snapshot => snapshot.Day == day);
        if (existedSnapshot is not null)
        {
            existedSnapshot.Update(claimedInUsd, pendingInUsd);
            return;
        }

        var snapshot = new MerklCampaignSnapshot(day, claimedInUsd, pendingInUsd, reason, CampaignId);

        _snapshots.Add(snapshot);
    }
}