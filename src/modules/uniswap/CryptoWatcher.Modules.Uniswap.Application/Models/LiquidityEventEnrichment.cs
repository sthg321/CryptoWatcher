using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public class LiquidityEventEnrichment
{
    public required DateTimeOffset TimeStamp { get; set; }

    public required TokenPair TokenPair { get; set; } = null!;
}