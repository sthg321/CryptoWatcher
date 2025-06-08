using CryptoWatcher.Application.Uniswap;
using CryptoWatcher.Core;
using CryptoWatcher.Entities;
using CryptoWatcher.Entities.Uniswap;
using CryptoWatcher.Integrations;
using CryptoWatcher.Models;
using CryptoWatcher.PoolHistorySyncFeature;
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
        _logger.LogInformation("Starting pool history synchronization");

        var wallets = await _repositoryFacade.GetWalletsAsync(ct);
        var networks = await _repositoryFacade.GetNetworksAsync(ct);

        _logger.LogInformation("Found {WalletCount} wallets and {NetworkCount} networks to process",
            wallets.Count, networks.Count);

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
                    _logger.LogInformation("No positions found for wallet {WalletAddress} on uniswapNetwork {NetworkName}",
                        wallet.Address, network.Name);
                    continue;
                }

                _logger.LogInformation(
                    "Found {PositionCount} positions for wallet {WalletAddress} on uniswapNetwork {NetworkName}",
                    uniswapPositions.Count, wallet.Address, network.Name);

                var existedPositions = (await _repositoryFacade.GetLiquidityPoolPositionsAsync(network, wallet, ct))
                    .ToDictionary(position => position.PositionId);

                var positions = new List<PoolPosition>();
                var poolPositionSnapshots = new List<PositionFee>();

                foreach (var uniswapPosition in uniswapPositions)
                {
                    using var positionScope = _logger.BeginScope("Processing position {PositionId}",
                        uniswapPosition.PositionId);

                    try
                    {
                        if (existedPositions.TryGetValue((ulong)uniswapPosition.PositionId,
                                out var existedPosition) &&
                            !existedPosition.IsActive)
                        {
                            _logger.LogInformation("Skipping inactive position");
                            continue;
                        }

                        var pool = await _providerFactory.GetPoolAsync(network, uniswapPosition);
                        var positionInPool = _math.CalculatePosition(pool, uniswapPosition);
                        var fee = _math.CalculateClaimableFee(pool, uniswapPosition);

                        var tokensEnriched = await _enricher.EnrichAsync(web3, positionInPool.TokenInfoPair, ct);
                        var feeEnriched = await _enricher.EnrichAsync(web3, fee, ct);

                        var positionEntity =
                            MapToLiquidityPoolPosition(network, wallet, uniswapPosition, tokensEnriched,
                                positionInPool.IsInRange);
                        
                        var snapshotEntity = MapToLiquidityPoolPositionSnapshot(
                            network, uniswapPosition, pool, feeEnriched);

                        positions.Add(positionEntity);
                        poolPositionSnapshots.Add(snapshotEntity);

                        _logger.LogInformation("Successfully synchronized position");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Failed to process position {PositionId} on uniswapNetwork {NetworkName} for wallet {WalletAddress}: {ErrorMessage}",
                            uniswapPosition.PositionId, network.Name, wallet.Address, ex.Message);
                    }
                }

                try
                {
                    await _repositoryFacade.MergePoolPositionsAsync(positions, poolPositionSnapshots, ct);

                    _logger.LogInformation(
                        "Persisted {PositionCount} positions and {SnapshotCount} snapshots for uniswapNetwork {NetworkName}",
                        positions.Count, poolPositionSnapshots.Count, network.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to save positions/snapshots to database for uniswapNetwork {NetworkName}: {ErrorMessage}",
                        network.Name, ex.Message);
                }

                _logger.LogInformation("Completed processing uniswapNetwork {NetworkName} for wallet {WalletAddress}",
                    network.Name, wallet.Address);
            }

            _logger.LogInformation("Completed processing wallet {WalletAddress}", wallet.Address);
        }

        _logger.LogInformation("Pool history synchronization completed successfully");
    }


    private static PoolPosition MapToLiquidityPoolPosition(UniswapNetwork uniswapNetwork, Wallet wallet,
        IUniswapPosition position, TokenInfoPair tokensEnriched, bool isInRage)
    {
        return new PoolPosition
        {
            NetworkName = uniswapNetwork.Name,
            SynchronizedAt = DateOnly.FromDateTime(DateTime.Now),
            IsActive = position.Liquidity != 0,
            Token0 = tokensEnriched.Token0,
            Token1 = tokensEnriched.Token1,
            WalletAddress = wallet.Address,
            IsInRange = isInRage,
            PositionId = (ulong)position.PositionId
        };
    }

    private static PositionFee MapToLiquidityPoolPositionSnapshot(
        UniswapNetwork uniswapNetwork,
        IUniswapPosition position,
        LiquidityPool pool,
        TokenInfoPair feeInfo)
    {
        return new PositionFee
        {
            Day = DateOnly.FromDateTime(DateTime.Now),
            Token0Fee = feeInfo.Token0,
            Token1Fee = feeInfo.Token1,
            IsInRange = pool.Tick >= position.TickLower && pool.Tick < position.TickUpper,
            LiquidityPoolPositionId = (ulong)position.PositionId,
            NetworkName = uniswapNetwork.Name,
        };
    }
}