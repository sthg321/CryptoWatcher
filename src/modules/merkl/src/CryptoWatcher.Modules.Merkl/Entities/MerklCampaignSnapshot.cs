using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Entities;

public class MerklCampaignSnapshot
{
    private MerklCampaignSnapshot()
    {
        
    }
    
    public MerklCampaignSnapshot(DateOnly day, decimal claimedInUsd, decimal pendingInUsd, string reason,
        TransactionHash merklCampaignId)
    {
        Day = day;
        ClaimedInUsd = claimedInUsd;
        PendingInUsd = pendingInUsd;
        Reason = reason;
        MerklCampaignId = merklCampaignId;
    }

    public DateOnly Day { get; private init; }

    public decimal ClaimedInUsd { get; private set; }

    public decimal PendingInUsd { get; private set; }

    public TransactionHash MerklCampaignId { get; private init; }

    /// <summary>
    /// Reason for reward.
    /// <example>UNISWAP_V3_0xD15965968fe8BF2BAbbe39b2FC5de1Ab6749141F_16469 
    /// UniswapV4_0xe56868928b91fcd5ebeada3d0ec8767f2bbfeb1e7da181203d13f6af76b03bf9_113779   
    /// MultiLogProcessor_11137654103842503226~MorphoVault_ERC20_0xBC03E505EE65f9fAa68a2D7e5A74452858C16D29~5478825671813402554_MorphoVaultV2_ERC20_0xbeeffeA75cFC4128ebe10C8D7aE22016D215060D
    /// Last value after underscore is position or vault id
    /// </example>
    /// </summary>
    public string Reason { get; private init; }

    public void Update(decimal claimedInUsd, decimal pendingInUsd)  
    {
        ClaimedInUsd = claimedInUsd;
        PendingInUsd = pendingInUsd;
    }
}