using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Infrastructure.Aave;

internal static partial class SyncAavePositionsCronJobLogs
{
    [LoggerMessage(LogLevel.Information, "Starting aave positions synchronization")]
    public static partial void SynchronizationStarted(this ILogger<SyncAavePositionsCronJob> logger);

    [LoggerMessage(LogLevel.Information, "Found {WalletCount} wallets to process")]
    public static partial void WalletsFound(this ILogger<SyncAavePositionsCronJob> logger, int walletCount);

    [LoggerMessage(LogLevel.Information, "Aave positions synchronization ended")]
    public static partial void SynchronizationCompleted(this ILogger<SyncAavePositionsCronJob> logger);
}