namespace CryptoWatcher.Models;

public record YieldMetrics(
    decimal TotalRoi,
    decimal NetFeeProfitUsd,
    decimal NetFeeRoi,
    decimal InitialValueUsd,
    decimal CurrentValueUsd,
    decimal FeeValueUsd
);