using System.Numerics;
using CryptoWatcher.Modules.Merkl.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Application.Models;

public class MerklCampaignInfo
{
    public int ChainId { get; init; }

    public BigInteger Amount { get; init; }
    
    public BigInteger Claimed { get; init; }

    public BigInteger Pending { get; init; }

    public TransactionHash CampaignId { get; init; } = null!;

    public string Reason { get; init; } = null!;

    public Asset Asset { get; init; } = null!;
}