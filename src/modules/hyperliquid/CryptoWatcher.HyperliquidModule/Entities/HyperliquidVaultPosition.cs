using CryptoWatcher.Extensions;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.HyperliquidModule.Entities;

/// <summary>
/// Represents a user's position within a Hyperliquid vault.
/// This class encapsulates various properties and methods to track and analyze the performance of the vault,
/// including event history, position snapshots, and profit calculations over specified time periods.
/// </summary>
public class HyperliquidVaultPosition
{
    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// Represents the collection of events associated with the vault's activity.
    /// </summary>
    /// <remarks>
    /// This property maintains a list of historical events that occurred within the vault.
    /// Each event describes a specific action, such as deposits or withdrawals, along with relevant details.
    /// It is utilized to analyze vault performance, track cash flow, and compute metrics like percentage profit or rate of return.
    /// </remarks>
    public List<HyperliquidVaultEvent> VaultEvents { get; init; } = [];

    /// <summary>
    /// 
    /// </summary>
    public List<HyperliquidVaultPositionSnapshot> PositionSnapshots { get; init; } = [];

    /// <summary>
    /// Calculates the percentage profit of the vault position within the specified date range.
    /// </summary>
    /// <param name="startDate">The start date of the date range for the calculation.</param>
    /// <param name="endDate">The end date of the date range for the calculation.</param>
    /// <returns>The percentage profit as a decimal value. Returns 0 if the date range is invalid or no data is available.</returns>
    public Percent CalculatePercentageProfit(DateOnly startDate, DateOnly endDate)
    {
        // Находим первый и последний снимки за период
        var startSnapshot = PositionSnapshots.GetNearestSnapshot(startDate, false);

        var endSnapshot = PositionSnapshots.GetNearestSnapshot(endDate, true);

        if (startSnapshot == null || endSnapshot == null || startSnapshot.Day >= endSnapshot.Day)
        {
            return 0;
        }

        var netCashFlow = VaultEvents.CalculateNetCashFlowInUsd(startSnapshot.Day, endSnapshot.Day);

        var positionChange = endSnapshot.Balance - startSnapshot.Balance - netCashFlow;

        if (startSnapshot.Balance == 0)
        {
            return 0;
        }

        return positionChange / startSnapshot.Balance;
    }

    /// <summary>
    /// Calculates the absolute profit of the vault position within the specified date range.
    /// </summary>
    /// <param name="startDate">The start date of the date range for the calculation.</param>
    /// <param name="endDate">The end date of the date range for the calculation.</param>
    /// <returns>The absolute profit as a decimal value. Returns 0 if the date range is invalid or no data is available.</returns>
    public decimal CalculateAbsoluteProfit(DateOnly startDate, DateOnly endDate)
    {
        var startSnapshot = PositionSnapshots.GetNearestSnapshot(startDate, false);
        var endSnapshot = PositionSnapshots.GetNearestSnapshot(endDate, true);

        if (startSnapshot == null || endSnapshot == null) return 0;

        var netCashFlow = VaultEvents.CalculateNetCashFlowInUsd(startSnapshot.Day, endSnapshot.Day);

        return endSnapshot.Balance - startSnapshot.Balance - netCashFlow;
    }
}