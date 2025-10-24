namespace CryptoWatcher.Modules.Uniswap.Entities;

public class UniswapLiquidityPositionDailyMetrics
{
    public ulong LiquidityPositionId { get; set; }

    public string NetworkName { get; set; } = null!;

    public DateOnly Day { get; set; }
    
    public decimal HoldValueInUsd { get; set; }

    public decimal CurrentValueInUsd { get; set; }

    public decimal CommissionInUsd { get; set; }
}