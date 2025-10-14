using System.Numerics;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services.PositionsSynchronization;

internal static partial class UniswapPositionsSyncServiceLogs
{
    [LoggerMessage(Level = LogLevel.Information, Message =
        "No positions found for wallet {WalletAddress} on uniswapNetwork {NetworkName}")]
    public static partial void NoPositionsFound(this ILogger logger, string walletAddress, string networkName);

    [LoggerMessage(LogLevel.Information,
        "Found {PositionCount} positions for wallet {WalletAddress} on uniswapNetwork {NetworkName}")]
    public static partial void PositionsFound(this ILogger logger, int positionCount, string walletAddress,
        string networkName);

    [LoggerMessage(LogLevel.Information, "Skipping inactive position")]
    public static partial void SkippingInactivePosition(this ILogger logger);

    [LoggerMessage(LogLevel.Information, "Successfully synchronized position")]
    public static partial void PositionSynchronizedSuccessfully(this ILogger logger);

    [LoggerMessage(LogLevel.Error,
        "Failed to process position {PositionId} on uniswapNetwork {NetworkName} for wallet {WalletAddress}")]
    public static partial void PositionProcessingFailed(this ILogger logger, BigInteger positionId,
        string networkName,
        string walletAddress, Exception ex);

    [LoggerMessage(LogLevel.Information,
        "Persisted {PositionCount} positions and {SnapshotCount} snapshots for uniswapNetwork {NetworkName}")]
    public static partial void PositionsPersisted(this ILogger logger, int positionCount, int snapshotCount,
        string networkName);

    [LoggerMessage(LogLevel.Error,
        "Failed to save positions/snapshots to database for uniswapNetwork {NetworkName}")]
    public static partial void PositionsSaveFailed(this ILogger logger, string networkName, Exception ex);

    [LoggerMessage(LogLevel.Information,
        "Completed processing uniswapNetwork {NetworkName} for wallet {WalletAddress}")]
    public static partial void
        NetworkProcessingCompleted(this ILogger logger, string networkName, string walletAddress);
}