using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserNonFundingLedgerUpdates;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserVaultEquities;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client;

public interface IHyperliquidApiClient
{
    IUserNonFundingLedgerUpdatesClient UserNonFundingLedgerUpdates { get; }

    IUserVaultEquitiesClient UserVaultEquities { get; }
}

public class HyperliquidApiClient : IHyperliquidApiClient
{
    public HyperliquidApiClient(IUserVaultEquitiesClient userVaultEquities,
        IUserNonFundingLedgerUpdatesClient userNonFundingLedgerUpdates)
    {
        UserVaultEquities = userVaultEquities;
        UserNonFundingLedgerUpdates = userNonFundingLedgerUpdates;
    }

    public IUserNonFundingLedgerUpdatesClient UserNonFundingLedgerUpdates { get; }

    public IUserVaultEquitiesClient UserVaultEquities { get; }
}