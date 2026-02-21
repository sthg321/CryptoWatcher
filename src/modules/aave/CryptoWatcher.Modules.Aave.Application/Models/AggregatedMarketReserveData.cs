using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Models;

public class AggregatedMarketReserveData
{
    public required EvmAddress UnderlyingAsset { get; init; } = null!;
    
    public required BigInteger Decimals { get; init; }
    
    public required BigInteger LiquidityIndex { get; init; }
    
    public required BigInteger VariableBorrowIndex { get; init; }
    
    public required BigInteger LiquidationLtv { get; init; }
    
    public required BigInteger PriceInMarketReferenceCurrency { get; init; }
    
    public required ushort ReserveLiquidationThreshold { get; init; }
}