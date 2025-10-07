using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.UniswapModule.Models;
using CryptoWatcher.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

/// <summary>
/// Defines a contract for synchronizing Uniswap positions for a given wallet, network, and date.
/// </summary>
public interface IUniswapPositionsSyncService
{
    /// <summary>
    /// Synchronizes Uniswap positions for a specified wallet and network on a given day.
    /// </summary>
    /// <param name="wallet">The cryptocurrency wallet for which to synchronize Uniswap positions.</param>
    /// <param name="chainConfiguration">The Uniswap network to connect to for synchronization.</param>
    /// <param name="syncDay">The date for which the Uniswap positions should be synchronized.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SyncUniswapPositionsAsync(Wallet wallet, UniswapChainConfiguration chainConfiguration, DateOnly syncDay,
        CancellationToken ct = default);
}

/// <summary>
/// <see cref="IUniswapPositionsSyncService"/>
/// </summary>
internal class UniswapPositionsSyncService : IUniswapPositionsSyncService
{
    private readonly record struct PositionKey(ulong PositionId, string NetworkName);

    private readonly ITokenEnricher _enricher;
    private readonly IUniswapProvider _providerFactory;
    private readonly IUniswapMath _math;
    private readonly IPoolHistorySyncRepositoryFacade _repositoryFacade;
    private readonly ILogger<UniswapPositionsSyncService> _logger;

    public UniswapPositionsSyncService(
        ITokenEnricher enricher,
        IUniswapProvider providerFactory,
        IUniswapMath math,
        IPoolHistorySyncRepositoryFacade repositoryFacade,
        ILogger<UniswapPositionsSyncService> logger)
    {
        _enricher = enricher;
        _providerFactory = providerFactory;
        _math = math;
        _logger = logger;
        _repositoryFacade = repositoryFacade;
    }

    public async Task SyncUniswapPositionsAsync(Wallet wallet, UniswapChainConfiguration chainConfiguration, DateOnly syncDay,
        CancellationToken ct = default)
    {
        var uniswapPositions = await _providerFactory.GetPositionsAsync(chainConfiguration, wallet);

        if (uniswapPositions.Count == 0)
        {
            _logger.NoPositionsFound(wallet.Address, chainConfiguration.Name);
            return;
        }

        _logger.PositionsFound(uniswapPositions.Count, wallet.Address, chainConfiguration.Name);

        var existedPositions = (await _repositoryFacade.GetLiquidityPoolPositionsAsync(chainConfiguration, wallet, ct))
            .ToDictionary(position => new PositionKey(position.PositionId, position.NetworkName));

        var positions = new List<PoolPosition>();
        var poolPositionSnapshots = new List<PoolPositionSnapshot>();

        foreach (var uniswapPosition in uniswapPositions)
        {
            using var positionScope = _logger.BeginScope("Processing position {PositionId}",
                uniswapPosition.PositionId);

            try
            {
                var pool = await _providerFactory.GetPoolAsync(chainConfiguration, uniswapPosition);

                var positionInPool = _math.CalculatePosition(pool, uniswapPosition);

                var tokensEnriched = await _enricher.EnrichAsync(chainConfiguration.RpcUrl, positionInPool.TokenInfoPair, ct);

                var positionKey = new PositionKey((ulong)uniswapPosition.PositionId, chainConfiguration.Name);
                if (!existedPositions.TryGetValue(positionKey, out var dbPoolPosition) ||
                    dbPoolPosition.PoolPositionSnapshots.Count == 1)
                    // for case when position was created
                    // and added liquidity in 1 day
                {
                    dbPoolPosition =
                        MapToLiquidityPoolPosition(chainConfiguration, wallet, uniswapPosition, tokensEnriched);
                    positions.Add(dbPoolPosition);
                }

                if (!dbPoolPosition.IsActive)
                {
                    _logger.SkippingInactivePosition();
                    continue;
                }

                var feeEnriched = await CalculateFeeAsync(chainConfiguration, pool, uniswapPosition, ct);

                var snapshotEntity = MapToLiquidityPoolPositionSnapshot(dbPoolPosition.PositionId,
                    dbPoolPosition.NetworkName, tokensEnriched, feeEnriched, positionInPool.IsInRange, syncDay);

                poolPositionSnapshots.Add(snapshotEntity);

                _logger.PositionSynchronizedSuccessfully();
            }
            catch (Exception ex)
            {
                _logger.PositionProcessingFailed(uniswapPosition.PositionId, chainConfiguration.Name, wallet.Address, ex);
            }
        }

        try
        {
            await _repositoryFacade.MergePoolPositionsAsync(positions, poolPositionSnapshots, ct);

            _logger.PositionsPersisted(positions.Count, poolPositionSnapshots.Count, chainConfiguration.Name);
        }
        catch (Exception ex)
        {
            _logger.PositionsSaveFailed(chainConfiguration.Name, ex);
        }

        _logger.NetworkProcessingCompleted(chainConfiguration.Name, wallet.Address);
    }

    private async Task<TokenInfoPair> CalculateFeeAsync(UniswapChainConfiguration chain, LiquidityPool pool,
        IUniswapPosition uniswapPosition, CancellationToken ct)
    {
        var fee = _math.CalculateClaimableFee(pool, uniswapPosition);

        return await _enricher.EnrichAsync(chain.RpcUrl, fee, ct);
    }

    private static PoolPosition MapToLiquidityPoolPosition(UniswapChainConfiguration chain, Wallet wallet,
        IUniswapPosition position, TokenInfoPair tokensEnriched)
    {
        return new PoolPosition
        {
            NetworkName = chain.Name,
            IsActive = position.Liquidity != 0,
            Token0 = tokensEnriched.Token0,
            Token1 = tokensEnriched.Token1,
            WalletAddress = wallet.Address,
            PositionId = (ulong)position.PositionId,
            ProtocolVersion = chain.ProtocolVersion,
            TickLower = position.TickLower,
            TickUpper = position.TickUpper
        };
    }

    private static PoolPositionSnapshot MapToLiquidityPoolPositionSnapshot(
        ulong positionId,
        string networkName,
        TokenInfoPair poolPosition,
        TokenInfoPair feeInfo,
        bool isInRange,
        DateOnly day)
    {
        return new PoolPositionSnapshot
        {
            PoolPositionId = positionId,
            NetworkName = networkName,
            Day = day,
            Token0 = TokenInfoWithFee.Create(poolPosition.Token0, feeInfo.Token0.Amount, feeInfo.Token0.PriceInUsd),
            Token1 = TokenInfoWithFee.Create(poolPosition.Token1, feeInfo.Token1.Amount, feeInfo.Token1.PriceInUsd),
            IsInRange = isInRange,
        };
    }
}