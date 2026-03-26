using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;

public record FluidFTokenData
{
    public required EvmAddress Liquidity { get; init; }
    
    public required EvmAddress LendingFactory { get; init; }
    
    public required EvmAddress LendingRewardsRateModel { get; init; }
    
    public required EvmAddress Permit2 { get; init; }
    
    public required EvmAddress Rebalancer { get; init; }
    
    public required bool RewardsActive { get; init; }
    
    public required BigInteger LiquidityBalance { get; init; }
    
    public required BigInteger LiquidityExchangePrice { get; init; }
    
    public required BigInteger TokenExchangePrice { get; init; }
}