using CryptoWatcher.Modules.Merkl.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Entities;

public class MerklCampaign
{
    private readonly List<MerklCampaignSnapshot> _snapshots = [];

    private MerklCampaign()
    {
    }

    public MerklCampaign(int chainId, EvmAddress walletAddress, string reason, string tokenSymbol,
        EvmAddress tokenAddress)
    {
        Id = Guid.CreateVersion7();
        ChainId = chainId;
        WalletAddress = walletAddress;
        Reason = reason;
        TokenSymbol = tokenSymbol;
        TokenAddress = tokenAddress;
    }

    public Guid Id { get; private set; }

    public int ChainId { get; private set; }

    public EvmAddress WalletAddress { get; private set; }

    public string TokenSymbol { get; private set; }

    public EvmAddress TokenAddress { get; private set; }

    /// <summary>
    /// Reason for reward.
    /// <example>UNISWAP_V3_0xD15965968fe8BF2BAbbe39b2FC5de1Ab6749141F_16469 
    /// UniswapV4_0xe56868928b91fcd5ebeada3d0ec8767f2bbfeb1e7da181203d13f6af76b03bf9_113779   
    /// MultiLogProcessor_11137654103842503226~MorphoVault_ERC20_0xBC03E505EE65f9fAa68a2D7e5A74452858C16D29~5478825671813402554_MorphoVaultV2_ERC20_0xbeeffeA75cFC4128ebe10C8D7aE22016D215060D
    /// Last value after underscore is position or vault id
    /// </example>
    /// </summary>
    public string Reason { get; private init; }

    public IReadOnlyCollection<MerklCampaignSnapshot> Snapshots => _snapshots;

    public void AddOrdUpdateSnapshot(DateOnly day, RewardStatus rewardStatus, decimal priceInUsd)
    {
        var existedSnapshot = _snapshots.FirstOrDefault(snapshot => snapshot.Day == day);
        if (existedSnapshot is not null)
        {
            existedSnapshot.Update(rewardStatus, priceInUsd);
            return;
        }

        var snapshot = new MerklCampaignSnapshot(day, rewardStatus, priceInUsd, Id);

        _snapshots.Add(snapshot);
    }

    public bool IsUniswapRewards() => Reason.StartsWith("UNISWAP_V3") || Reason.StartsWith("UniswapV4");

    public ulong GetUniswapId()
    {
        var lastSlashIndex = Reason.LastIndexOf('_');

        return ulong.Parse(Reason[(lastSlashIndex + 1)..]);
    }
}