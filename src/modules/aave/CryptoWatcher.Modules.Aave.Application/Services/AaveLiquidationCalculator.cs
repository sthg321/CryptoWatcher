using CryptoWatcher.Modules.Aave.Entities;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public class AaveLiquidationCalculator
{
    /// <summary>
    /// Суммарная стоимость залога в USD при которой наступает ликвидация.
    /// Предполагается что пропорции между активами сохраняются.
    ///
    /// Формула:
    ///   weighted_avg_lt           = Σ(amount_i × price_i × LT_i) / Σ(amount_i × price_i)
    ///   account_liquidation_value = total_debt_usd / weighted_avg_lt
    ///
    /// Возвращает null если нет залогов или нет долга.
    /// </summary>
    public decimal? AccountLiquidationValue(decimal totalDebtUsd, IReadOnlyCollection<AavePositionSnapshot> supplies)
    {
        if (totalDebtUsd <= 0) return null;

        var totalCollateralUsd = TotalCollateralUsd(supplies);
        if (totalCollateralUsd <= 0) return null;

        var weightedAvgLt = WeightedAverageLt(supplies, totalCollateralUsd);

        return totalDebtUsd / weightedAvgLt;
    }

    /// <summary>
    /// Суммарная стоимость всех Supply активов в USD.
    ///
    /// Формула:
    ///   total_collateral_usd = Σ(amount_i × price_i)
    /// </summary>
    private static decimal TotalCollateralUsd(IReadOnlyCollection<AavePositionSnapshot> supplies)
        => supplies.Sum(s => s.Token0.AmountInUsd);

    /// <summary>
    /// Средневзвешенный Liquidation Threshold по всем Supply активам.
    ///
    /// Формула:
    ///   weighted_avg_lt = Σ(amount_i × price_i × LT_i) / total_collateral_usd
    /// </summary>
    private static decimal WeightedAverageLt(IReadOnlyCollection<AavePositionSnapshot> supplies,
        decimal totalCollateralUsd)
        => supplies.Sum(s => s.Token0.AmountInUsd * (decimal)s.LiquidationLtv!.Value) / totalCollateralUsd;
}