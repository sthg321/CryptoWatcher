using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Aave.Application.Services;

internal static partial class AavePositionsSyncServiceLogs
{
    [LoggerMessage(LogLevel.Information,
        "For wallet address: {WalletAddress} found {AavePositionsCount} existed aave positions")]
    public static partial void LogExistedPositionsForWalletCount(this ILogger<AavePositionsSyncService> log,
        string walletAddress, int aavePositionsCount);

    [LoggerMessage(LogLevel.Information,
        "For network: {AaveNetwork} found {AavePositionsCount} positions")]
    public static partial void LogFetchedPositionsForNetworkCount(this ILogger<AavePositionsSyncService> log,
        string aaveNetwork, int aavePositionsCount);

    [LoggerMessage(LogLevel.Information,
        "Position id: {PositionId} with {TokenAddress} is closed on aave. Will be marked as closed at")]
    public static partial void LogPositionClosed(this ILogger<AavePositionsSyncService> log,
        Guid positionId, string tokenAddress);

    [LoggerMessage(LogLevel.Information,
        "For token address: {AaveTokenAddress} position not exists. Will be created with token @{cryptoToken}")]
    public static partial void LogCreateAavePosition(this ILogger<AavePositionsSyncService> log,
        string aaveTokenAddress, CryptoToken cryptoToken);

    [LoggerMessage(LogLevel.Information,
        "For token address: {AaveTokenAddress} position exists. Will be update with token @{cryptoToken}")]
    public static partial void LogUpdateAavePosition(this ILogger<AavePositionsSyncService> log,
        string aaveTokenAddress, CryptoToken cryptoToken);
}