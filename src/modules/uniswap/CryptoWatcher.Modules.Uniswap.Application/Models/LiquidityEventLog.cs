using System.Numerics;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public record LiquidityEventLog
{
    public string Address { get; init; } = null!;

    public string[] Topics { get; init; } = null!;

    public BigInteger Data { get; init; }
}