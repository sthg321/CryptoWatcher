using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.UserNonFundingLedgerUpdates;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.UserVaultEquities;
using Refit;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Api;

public interface IHyperliquidApi
{
    /// <summary>
    /// Retrieve a user's funding history or non-funding ledger updates
    /// </summary>
    /// <remarks>https://hyperliquid.gitbook.io/hyperliquid-docs/for-developers/api/info-endpoint/perpetuals#request-body-4</remarks>
    /// <returns></returns>
    [Post("/info")]
    Task<UserNonFundingLedgerUpdate[]> GetUserNonFundingLedgerUpdatesAsync(UserNonFundingLedgerUpdatesRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Retrieve a user's vault balance
    /// </summary>
    /// <remarks>https://hyperliquid.gitbook.io/hyperliquid-docs/for-developers/api/info-endpoint?q=userFills#retrieve-a-users-vault-deposits</remarks>
    /// <returns></returns>
    [Post("/info")]
    Task<UserVaultEquity[]> GetUserVaultEquitiesAsync(UserVaultEquitiesRequest request,
        CancellationToken ct = default);
}