using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    Models.PositionEvents;

public abstract class PositionEvent
{
    public required ulong PositionId { get; init; }

    public required EvmAddress From { get; init; } = null!;

    public required TransactionHash TransactionHash { get; init; } = null!;

    public Token Token0 { get; init; } = null!;

    public Token Token1 { get; init; } = null!;

    public required BigInteger BlockNumber { get; init; }
}