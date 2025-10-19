using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public class BlockscoutTransaction
{
    public string? Method { get; init; }

    public required TransactionHash TransactionHash { get; init; } = null!;

    public required BigInteger BlockNumber { get; init; }
}