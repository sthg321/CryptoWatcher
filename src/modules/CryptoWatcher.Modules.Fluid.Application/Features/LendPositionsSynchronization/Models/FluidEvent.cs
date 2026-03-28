using System.Numerics;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;

public record FluidEvent
{
    public Token Token { get; init; } = null!;
    
    /// <summary>
    /// Неиндексированный параметр shares (uint256)
    /// </summary>
    public BigInteger Shares { get; init; }

    public CashFlowEvent EventType { get; init; } = null!;
}