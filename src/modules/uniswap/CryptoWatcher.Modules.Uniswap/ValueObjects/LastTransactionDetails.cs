using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.ValueObjects;

public class LastTransactionDetails
{
    public TransactionHash LastTransactionHash { get; set; } = null!;

    public long LastBlockNumber { get; set; }
}