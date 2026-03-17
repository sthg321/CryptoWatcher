using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Contracts.Messages;

public class BlockchainTransaction
{
    public string? FunctionName { get; init; } = null!;

    public required int ChainId { get; init; } 
    
    public required TransactionHash Hash { get; init; } = null!;

    public required EvmAddress From { get; init; } = null!;
    
    public required EvmAddress To { get; init; } = null!;

    public required BigInteger BlockNumber { get; init; }

    public required DateTime Timestamp { get; init; }
}