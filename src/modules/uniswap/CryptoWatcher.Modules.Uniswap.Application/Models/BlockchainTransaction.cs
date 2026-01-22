using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public class BlockchainTransaction
{
    public required string FunctionName { get; set; } = null!;

    public required TransactionHash Hash { get; set; } = null!;

    public required EvmAddress To { get; set; } = null!;

    public required DateTime Timestamp { get; set; }
}