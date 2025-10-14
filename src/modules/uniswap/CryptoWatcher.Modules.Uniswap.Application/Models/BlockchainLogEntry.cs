using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public record BlockchainLogEntry
{
    public required TransactionHash TransactionHash { get; init; } = null!;

    public required string Data { get; init; } = null!;

    public required EvmAddress Address { get; init; } = null!;
}