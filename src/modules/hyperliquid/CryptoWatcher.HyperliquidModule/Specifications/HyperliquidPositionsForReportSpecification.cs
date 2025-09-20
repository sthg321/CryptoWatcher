using Ardalis.Specification;
using CryptoWatcher.HyperliquidModule.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.HyperliquidModule.Specifications;

/// <summary>
/// Represents a specification to retrieve Hyperliquid vault positions within a specific date range
/// for generating reports. This specification facilitates querying the necessary
/// data by including related vault events and filtering position snapshots that fall
/// between the specified start and end dates.
/// </summary>
internal sealed class HyperliquidPositionsForReportSpecification : Specification<HyperliquidVaultPosition>
{
    public HyperliquidPositionsForReportSpecification(IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to)
    {
        var valetAddresses = wallets.Select(wallet => wallet.Address).ToArray();
        Query
            .Where(position => valetAddresses.Contains(position.WalletAddress))
            .Include(position => position.Wallet)
            .Include(position => position.VaultEvents)
            .Include(position => position.PositionSnapshots
                .OrderBy(snapshot => snapshot.Day)
                .Where(snapshot => snapshot.Day >= from && snapshot.Day <= to));
    }
}