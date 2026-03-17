using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.WalletIngestion.Entities;

public class WalletIngestionCheckpoint
{
    private WalletIngestionCheckpoint()
    {
    }

    private WalletIngestionCheckpoint(EvmAddress walletAddress, int chainId)
    {
        WalletAddress = walletAddress;
        ChainId = chainId;
        UpdatedAt = DateTime.UtcNow;
    }

    public EvmAddress WalletAddress { get; private set; } = null!;

    public int ChainId { get; private set; }

    public TransactionHash? LastPublishedTransactionHash { get; private set; }

    public BigInteger LastPublishedBlockNumber { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public static WalletIngestionCheckpoint Create(EvmAddress walletAddress, int chainId)
    {
        return new WalletIngestionCheckpoint(walletAddress, chainId);
    }
 
    public void Advance(TransactionHash lastHash, BigInteger lastBlockNumber, TimeProvider timeProvider)
    {
        LastPublishedTransactionHash = lastHash;
        LastPublishedBlockNumber = lastBlockNumber;
        UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;
    }
}