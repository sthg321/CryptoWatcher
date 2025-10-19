using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public class BlockscoutNextPageParams
{
    public int Index { get; init; }

    public TransactionHash Hash { get; init; } = null!;

    public BigInteger BlockNumber { get; init; }
}