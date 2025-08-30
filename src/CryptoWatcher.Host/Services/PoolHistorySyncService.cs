using System.Numerics;
using CryptoWatcher.Integrations;
using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule;
using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.UniswapModule.Models;
using CryptoWatcher.UniswapModule.Services;
using Nethereum.Web3;
using UniswapClient.Models;

namespace CryptoWatcher.Host.Services;

public class PoolHistorySyncService
{
    private readonly TokenEnricher _enricher;
    private readonly IUniswapProvider _providerFactory;
    private readonly IUniswapMath _math;
    private readonly IPoolHistorySyncRepositoryFacade _repositoryFacade;
    private readonly ILogger<PoolHistorySyncService> _logger;

    public PoolHistorySyncService(
        TokenEnricher enricher,
        IUniswapProvider providerFactory,
        IUniswapMath math,
        IPoolHistorySyncRepositoryFacade repositoryFacade,
        ILogger<PoolHistorySyncService> logger)
    {
        _enricher = enricher;
        _providerFactory = providerFactory;
        _math = math;
        _logger = logger;
        _repositoryFacade = repositoryFacade;
    }

    public async Task SyncAsync(CancellationToken ct = default)
    {
        _logger.StartingPoolSync();

        var wallets = await _repositoryFacade.GetWalletsAsync(ct);
        var networks = await _repositoryFacade.GetNetworksAsync(ct);

        _logger.WalletsAndNetworksCount(wallets.Count, networks.Count);

        foreach (var wallet in wallets)
        {
            using var walletScope = _logger.BeginScope("Processing wallet {WalletAddress}", wallet.Address);

            foreach (var network in networks)
            {
                using var networkScope = _logger.BeginScope("Processing uniswapNetwork {NetworkName}", network.Name);

                var web3 = new Web3(network.RpcUrl);
                var uniswapPositions = await _providerFactory.GetPositionsAsync(network, wallet);

                if (uniswapPositions.Count == 0)
                {
                    _logger.NoPositionsFound(wallet.Address, network.Name);
                    continue;
                }

                _logger.PositionsFound(uniswapPositions.Count, wallet.Address, network.Name);

                var existedPositions = (await _repositoryFacade.GetLiquidityPoolPositionsAsync(network, wallet, ct))
                    .ToDictionary(position => new PositionKey(position.PositionId, position.NetworkName));

                var positions = new List<PoolPosition>();
                var poolPositionSnapshots = new List<PoolPositionSnapshot>();

                foreach (var uniswapPosition in uniswapPositions)
                {
                    using var positionScope = _logger.BeginScope("Processing position {PositionId}",
                        uniswapPosition.PositionId);

                    try
                    {
                        var pool = await _providerFactory.GetPoolAsync(network, uniswapPosition);

                        var positionInPool = _math.CalculatePosition(pool, uniswapPosition);

                        var tokensEnriched = await _enricher.EnrichAsync(network, positionInPool.TokenInfoPair, ct);

                        var positionKey = new PositionKey((ulong)uniswapPosition.PositionId, network.Name);
                        if (!existedPositions.TryGetValue(positionKey, out var dbPoolPosition))
                        {
                            dbPoolPosition =
                                MapToLiquidityPoolPosition(network, wallet, uniswapPosition, tokensEnriched);
                            positions.Add(dbPoolPosition);
                        }

                        if (dbPoolPosition.PoolPositionSnapshots.Count == 1) // for case when position was created
                            // and added liquidity in 1 day
                        {
                            dbPoolPosition =
                                MapToLiquidityPoolPosition(network, wallet, uniswapPosition, tokensEnriched);
                            positions.Add(dbPoolPosition);
                        }

                        if (!dbPoolPosition.IsActive)
                        {
                            _logger.SkippingInactivePosition();
                            continue;
                        }

                        var fee = _math.CalculateClaimableFee(pool, uniswapPosition);

                        var feeEnriched = await _enricher.EnrichAsync(network, fee, ct);

                        var snapshotEntity = MapToLiquidityPoolPositionSnapshot(dbPoolPosition.PositionId,
                            dbPoolPosition.NetworkName, tokensEnriched, feeEnriched, positionInPool.IsInRange);

                        poolPositionSnapshots.Add(snapshotEntity);

                        _logger.PositionSynchronizedSuccessfully();
                    }
                    catch (Exception ex)
                    {
                        _logger.PositionProcessingFailed(uniswapPosition.PositionId, network.Name, wallet.Address, ex);
                    }
                }

                try
                {
                    await _repositoryFacade.MergePoolPositionsAsync(positions, poolPositionSnapshots, ct);

                    _logger.PositionsPersisted(positions.Count, poolPositionSnapshots.Count, network.Name);
                }
                catch (Exception ex)
                {
                    _logger.PositionsSaveFailed(network.Name, ex);
                }

                _logger.NetworkProcessingCompleted(network.Name, wallet.Address);
            }

            _logger.WalletProcessingCompleted(wallet.Address);
        }

        _logger.PoolSyncCompleted();
    }

    private static PoolPosition MapToLiquidityPoolPosition(UniswapNetwork uniswapNetwork, Wallet wallet,
        IUniswapPosition position, TokenInfoPair tokensEnriched)
    {
        return new PoolPosition
        {
            NetworkName = uniswapNetwork.Name,
            IsActive = position.Liquidity != 0,
            Token0 = tokensEnriched.Token0,
            Token1 = tokensEnriched.Token1,
            WalletAddress = wallet.Address,
            PositionId = (ulong)position.PositionId
        };
    }

    private static PoolPositionSnapshot MapToLiquidityPoolPositionSnapshot(
        ulong positionId,
        string networkName,
        TokenInfoPair poolPosition,
        TokenInfoPair feeInfo,
        bool isInRange)
    {
        return new PoolPositionSnapshot
        {
            PoolPositionId = positionId,
            NetworkName = networkName,
            Day = DateOnly.FromDateTime(DateTime.Now),
            Token0 = TokenInfoWithFee.Create(poolPosition.Token0, feeInfo.Token0.Amount, feeInfo.Token0.PriceInUsd),
            Token1 = TokenInfoWithFee.Create(poolPosition.Token1, feeInfo.Token1.Amount, feeInfo.Token1.PriceInUsd),
            IsInRange = isInRange,
        };
    }

    private readonly record struct PositionKey(ulong PositionId, string NetworkName);
}

public static partial class PoolSyncLogs
{
    [LoggerMessage(LogLevel.Information, "Starting pool history synchronization")]
    public static partial void StartingPoolSync(this ILogger logger);

    [LoggerMessage(LogLevel.Information, "Found {WalletCount} wallets and {NetworkCount} networks to process")]
    public static partial void WalletsAndNetworksCount(this ILogger logger, int walletCount, int networkCount);

    [LoggerMessage(LogLevel.Information,
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
    public static partial void PositionProcessingFailed(this ILogger logger, BigInteger positionId, string networkName,
        string walletAddress, Exception ex);

    [LoggerMessage(LogLevel.Information,
        "Persisted {PositionCount} positions and {SnapshotCount} snapshots for uniswapNetwork {NetworkName}")]
    public static partial void PositionsPersisted(this ILogger logger, int positionCount, int snapshotCount,
        string networkName);

    [LoggerMessage(LogLevel.Error, "Failed to save positions/snapshots to database for uniswapNetwork {NetworkName}")]
    public static partial void PositionsSaveFailed(this ILogger logger, string networkName, Exception ex);

    [LoggerMessage(LogLevel.Information,
        "Completed processing uniswapNetwork {NetworkName} for wallet {WalletAddress}")]
    public static partial void
        NetworkProcessingCompleted(this ILogger logger, string networkName, string walletAddress);

    [LoggerMessage(LogLevel.Information, "Completed processing wallet {WalletAddress}")]
    public static partial void WalletProcessingCompleted(this ILogger logger, string walletAddress);

    [LoggerMessage(LogLevel.Information, "Pool history synchronization completed successfully")]
    public static partial void PoolSyncCompleted(this ILogger logger);
}