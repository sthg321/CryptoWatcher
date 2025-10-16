namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserNonFundingLedgerUpdates.Contracts;

/// <summary>
/// Retrieve a user's vault deposits
/// </summary>
/// <param name="Type">userVaultEquities</param>
/// <param name="User">wallet address</param>
public record GetUserNonFundingLedgerUpdatesRequest(string Type, string User);