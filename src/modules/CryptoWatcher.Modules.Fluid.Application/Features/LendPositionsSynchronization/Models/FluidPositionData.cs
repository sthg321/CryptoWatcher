using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;

public class FluidPositionData
{
    public required EvmAddress FTokenAddress { get; init; }
    
    public required EvmAddress WalletAddress { get; init; }
    
    public required BigInteger SharesBalance { get; init; }
    
    public required BigInteger LiquidityExchangePrice { get; init; }
    
    public required BigInteger TokenExchangePrice { get; init; }
    
    public required BigInteger LiquidityBalance { get; init; }
    
    public required bool RewardsActive { get; init; }
 
    private const decimal ExchangePriceScale = 1_000_000_000_000m;
 
    public decimal CalculateUnderlyingBalance(int underlyingDecimals)
    {
        if (SharesBalance == BigInteger.Zero)
            return 0m;
 
        var divisor = BigInteger.Pow(10, underlyingDecimals);
        return (decimal)(SharesBalance * TokenExchangePrice) / (decimal)divisor / ExchangePriceScale;
    }
}