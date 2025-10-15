using CryptoWatcher.Models;

namespace CryptoWatcher.Modules.Hyperliquid.Models;

/// <summary>
/// Represents a report item for a specific vault in the Hyperliquid module.
/// </summary>
/// <remarks>
/// This record serves as a model to encapsulate the daily balance and profits associated with a vault.
/// </remarks>
public class HyperliquidVaultReportItem : PlatformDailyReportItem
{
    /// <summary>
    /// Gets the unique address of the vault associated with the report item.
    /// </summary>
    /// <remarks>
    /// This property identifies the address of the vault used within the Hyperliquid module
    /// and is critical for mapping vault-specific data, such as balances and profits, across reports.
    /// </remarks>
    public required string VaultAddress { get; init; } = null!;
}