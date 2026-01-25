using System.Numerics;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Entities;

public class UniswapSynchronizationState
{
    private UniswapSynchronizationState() // for ef
    {
        
    }
    
    public UniswapSynchronizationState(UniswapChainConfiguration chain, Wallet wallet,
        TransactionHash lastTransactionHash, BigInteger lastBlockNumber, TimeProvider provider)
    {
        ChainConfiguration = chain;
        ChainName = chain.Name;
        UniswapProtocolVersion = chain.ProtocolVersion;
        WalletAddress = wallet.Address;
        Wallet = wallet;

        LastTransactionHash = lastTransactionHash;
        LastBlockNumber = lastBlockNumber;
        SynchronizedAt = provider.GetUtcNow().UtcDateTime;
    }

    public UniswapSynchronizationState(UniswapChainConfiguration chain, Wallet wallet)
    {
         ChainConfiguration = chain;
         ChainName = chain.Name;
         UniswapProtocolVersion = chain.ProtocolVersion;
         LastBlockNumber = 0;
         WalletAddress = wallet.Address;
    }

    public TransactionHash? LastTransactionHash { get; private set; }

    public BigInteger LastBlockNumber { get; private set; }

    public EvmAddress WalletAddress { get; private set; }

    public Wallet Wallet { get; private set; } = null!;

    public DateTime SynchronizedAt { get; set; }

    public string ChainName { get; private set; }

    public UniswapProtocolVersion UniswapProtocolVersion { get; private set; }

    public UniswapChainConfiguration ChainConfiguration { get; private set; }

    public void UpdateLastSynchronizedTransaction(TransactionHash hash, BigInteger lastBlockNumber,
        TimeProvider provider)
    {
        LastTransactionHash = hash;
        LastBlockNumber = lastBlockNumber;
        SynchronizedAt = provider.GetUtcNow().UtcDateTime;
    }
}