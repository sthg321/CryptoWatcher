using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public class BlockchainTransaction
{
    public string? FunctionName { get; init; } = null!;

    public required TransactionHash Hash { get; init; } = null!;

    public required EvmAddress To { get; init; } = null!;

    public required BigInteger BlockNumber { get; init; }

    public required DateTime Timestamp { get; init; }
}