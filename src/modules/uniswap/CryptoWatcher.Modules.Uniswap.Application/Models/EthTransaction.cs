using System.Numerics;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public record EthTransaction
{
    public required BigInteger Amount { get; init; }

    public required DateTimeOffset TimeStamp { get; init; }
}