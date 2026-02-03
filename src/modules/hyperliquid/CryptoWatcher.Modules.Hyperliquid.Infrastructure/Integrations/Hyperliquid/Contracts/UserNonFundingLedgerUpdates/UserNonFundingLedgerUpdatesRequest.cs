namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.
    UserNonFundingLedgerUpdates;

/// <summary>
/// Retrieve a user's vault deposits
/// </summary>
public class UserNonFundingLedgerUpdatesRequest
{
    /// <summary>
    /// Retrieve a user's vault deposits
    /// </summary>
    /// <param name="user">Address in 42-character hexadecimal format; e.g. 0x0000000000000000000000000000000000000000.</param>
    /// <param name="startTime">Start time in milliseconds, inclusive</param>
    /// <param name="endTime">End time in milliseconds, inclusive. Defaults to current time.</param>
    public UserNonFundingLedgerUpdatesRequest(string user, long startTime, long endTime)
    {
        User = user;
        StartTime = startTime;
        EndTime = endTime;
        Type = "userNonFundingLedgerUpdates";
    }

    public string User { get; set; }

    public long StartTime { get; set; }

    public long EndTime { get; set; }

    public string Type { get; set; }
}