using CryptoWatcher.Extensions;

namespace CryptoWatcher.Entities.Hyperliquid;

public class HyperliquidVaultPosition
{
    public string VaultAddress { get; init; } = null!;

    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address linked to the liquidity pool position.
    /// It is used to identify the owner of the position and manage the related account details.
    /// </remarks>
    public string WalletAddress { get; init; } = null!;

    /// <summary>
    /// Represents the wallet associated with a liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property identifies the wallet that holds ownership of the liquidity pool position.
    /// It includes the wallet's unique identifier and blockchain address for managing assets.
    /// </remarks>
    public Wallet Wallet { get; init; } = null!;

    public List<HyperliquidVaultEvent> VaultEvents { get; init; } = [];

    public List<HyperliquidVaultPositionSnapshot> PositionSnapshots { get; init; } = [];
    
    /// <summary>
    /// Рассчитывает процентное изменение позиции за указанный период
    /// </summary>
    /// <param name="startDate">Начальная дата периода</param>
    /// <param name="endDate">Конечная дата периода</param>
    /// <returns>Процентное изменение позиции за период</returns>
    public decimal CalculatePercentageChange(DateOnly startDate, DateOnly endDate)
    {
        // Находим первый и последний снимки за период
        var firstSnapshot = GetNearestSnapshot(startDate, false);
        
        var lastSnapshot = GetNearestSnapshot(endDate, true);

        if (firstSnapshot == null || lastSnapshot == null || firstSnapshot.Day >= lastSnapshot.Day)
        {
            return 0;
        }
        
        var netCashFlow = VaultEvents
            .Where(e => e.Date >= firstSnapshot.Day.ToDateTime(TimeOnly.MinValue) && 
                        e.Date <= lastSnapshot.Day.ToDateTime(TimeOnly.MinValue)).Sum(e => 
            e.EventType == HyperliquidVaultVaultEventType.Deposit ? e.Usd : -e.Usd);

        var positionChange = lastSnapshot.Balance - firstSnapshot.Balance - netCashFlow;

        if (firstSnapshot.Balance == 0)
        {
            return 0;
        }
        
        var percentageChange = positionChange / firstSnapshot.Balance * 100;
        
        return percentageChange;
    }

    public decimal CalculateAbsoluteProfit(DateOnly startDate, DateOnly endDate)
    {
        var startSnapshot = GetNearestSnapshot(startDate, false);
        var endSnapshot = GetNearestSnapshot(endDate, true);

        if (startSnapshot == null || endSnapshot == null) return 0;

        var deposits = GetEventsSum(HyperliquidVaultVaultEventType.Deposit, startSnapshot.Day, endSnapshot.Day);

        var withdrawals = GetEventsSum(HyperliquidVaultVaultEventType.Withdrawal, startSnapshot.Day, endSnapshot.Day);

        return endSnapshot.Balance - startSnapshot.Balance - deposits + withdrawals;
    }

    public decimal CalculateRateOfReturn(DateOnly startDate, DateOnly endDate)
    {
        var startSnapshot = GetNearestSnapshot(startDate, false);
        var endSnapshot = GetNearestSnapshot(endDate, true);

        if (startSnapshot == null || endSnapshot == null) return 0;

        var periodEventsNetFlow = VaultEvents
            .Where(e => e.Date >= startDate.ToDateTime(TimeOnly.MinValue) &&
                        e.Date <= endDate.ToDateTime(TimeOnly.MinValue))
            .Sum(e => e.EventType == HyperliquidVaultVaultEventType.Deposit ? e.Usd : -e.Usd);

        return (endSnapshot.Balance - startSnapshot.Balance - periodEventsNetFlow) / startSnapshot.Balance;
    }

    private decimal GetEventsSum(HyperliquidVaultVaultEventType eventType, DateOnly from, DateOnly to)
    {
        return VaultEvents
            .Where(e => e.EventType == eventType &&
                        e.Date >= from.ToDateTime() &&
                        e.Date <= to.ToDateTime())
            .Sum(e => e.Usd);
    }

    private HyperliquidVaultPositionSnapshot? GetNearestSnapshot(DateOnly date, bool findPrevious)
    {
        if (findPrevious)
        {
            return PositionSnapshots
                .Where(s => s.Day <= date)
                .OrderByDescending(s => s.Day)
                .FirstOrDefault();
        }

        return PositionSnapshots
            .Where(s => s.Day >= date)
            .OrderBy(s => s.Day)
            .FirstOrDefault();
    }
}