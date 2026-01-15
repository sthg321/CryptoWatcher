using CryptoWatcher.Extensions;
using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Models.Reports;

public class UniswapOverallReport
{
    public string Network { get; init; } = null!;

    public string Pair { get; init; } = null!;

    public DateOnly CreatedAt { get; init; }

    public DateOnly? ClosedAt { get; init; }

    public Money InitialBalanceInUsd { get; init; }

    public Money CurrentBalanceInUsd { get; init; }

    public Money CommissionInUsd { get; init; }

    public Money RewardsInUsd { get; init; }

    public Money Pnl => CurrentBalanceInUsd - InitialBalanceInUsd + RewardsInUsd + CommissionInUsd;

    public Percent Apr => CalculateApr();

    private Percent CalculateApr()
    {
        if (InitialBalanceInUsd == 0)
        {
            return 0;
        }

        var lastDay = ClosedAt?.ToMinDateTime() ?? DateTime.UtcNow.Date;
        var positionActiveDays = (lastDay - CreatedAt.ToMinDateTime()).Days;
        if (positionActiveDays == 0)
        {
            return 0;
        }

        var annualizedReturn = (Pnl / InitialBalanceInUsd) * (365m / positionActiveDays);
        return annualizedReturn;
    }
}