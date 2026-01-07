using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.ValueObjects;

public class Asset
{
    public required string Symbol { get; init; } = null!;
    
    public required EvmAddress Address { get; init; } = null!;

    public required byte Decimals { get; init; }
    
    public required decimal PriceInUsd { get; init; }
}