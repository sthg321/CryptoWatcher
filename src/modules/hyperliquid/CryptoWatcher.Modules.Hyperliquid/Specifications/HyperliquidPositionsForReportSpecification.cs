using Ardalis.Specification;
using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Specifications;

/// <summary>
/// Represents a specification to retrieve Hyperliquid vault positions within a specific date range
/// for generating reports. This specification facilitates querying the necessary
/// data by including related vault events and filtering position snapshots that fall
/// between the specified start and end dates.
/// </summary>
public sealed class HyperliquidPositionsForReportSpecification : Specification<HyperliquidVaultPosition>
{
    public HyperliquidPositionsForReportSpecification(IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to)
    {
        var valetAddresses = wallets.Select(wallet => wallet.Address.Value).ToArray();
        Query
            .Where(position => valetAddresses.Contains(position.WalletAddress))
            .Include(position => position.Wallet)
            .Include(position =>
                position.VaultEvents.Where(@event => @event.Date.ToDateOnly() > from && @event.Date.ToDateOnly() <= to))
            .Include(position => position.PositionSnapshots
                .OrderBy(snapshot => snapshot.Day)
                .Where(snapshot => snapshot.Day >= from && snapshot.Day <= to));
    }
}