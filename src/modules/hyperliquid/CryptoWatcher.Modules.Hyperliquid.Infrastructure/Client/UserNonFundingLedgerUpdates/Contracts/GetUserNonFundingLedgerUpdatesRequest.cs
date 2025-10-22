namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserNonFundingLedgerUpdates.Contracts;

/// <summary>
/// Retrieve a user's vault deposits
/// </summary>
/// <param name="Type">"userFunding" or "userNonFundingLedgerUpdates"</param>
/// <param name="User">Address in 42-character hexadecimal format; e.g. 0x0000000000000000000000000000000000000000.</param>
/// <param name="User">Start time in milliseconds, inclusive</param>
/// <param name="User">End time in milliseconds, inclusive. Defaults to current time.</param>
public record GetUserNonFundingLedgerUpdatesRequest(string Type, string User, long StartTime, long EndTime);