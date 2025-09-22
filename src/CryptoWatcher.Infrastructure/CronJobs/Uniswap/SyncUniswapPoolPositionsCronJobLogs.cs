using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Infrastructure.CronJobs.Uniswap;

internal static partial class SyncUniswapPoolPositionsCronJobLogs
{
    [LoggerMessage(LogLevel.Information, "Starting pool history synchronization")]
    public static partial void StartingPoolSync(this ILogger logger);

    [LoggerMessage(LogLevel.Information, "Found {WalletCount} wallets and {NetworkCount} networks to process")]
    public static partial void WalletsAndNetworksCount(this ILogger logger, int walletCount, int networkCount);

    [LoggerMessage(LogLevel.Information, "Successfully synchronized position")]
    public static partial void PositionSynchronizedSuccessfully(this ILogger logger);

    [LoggerMessage(LogLevel.Information,
        "Completed processing uniswapNetwork {NetworkName} for wallet {WalletAddress}")]
    public static partial void
        NetworkProcessingCompleted(this ILogger logger, string networkName, string walletAddress);

    [LoggerMessage(LogLevel.Information, "Completed processing wallet {WalletAddress}")]
    public static partial void WalletProcessingCompleted(this ILogger logger, string walletAddress);

    [LoggerMessage(LogLevel.Information, "Pool history synchronization completed successfully")]
    public static partial void PoolSyncCompleted(this ILogger logger);
}