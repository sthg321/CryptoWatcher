using System.Runtime.CompilerServices;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsSnapshotSynchronization;

public class PositionSnapshotUpdater : IPositionSnapshotUpdater
{
    private readonly IUniswapProvider _uniswapProvider;
    private readonly IPositionEvaluator _positionEvaluator;
    private readonly ILogger<PositionSnapshotUpdater> _logger;

    public PositionSnapshotUpdater(IUniswapProvider uniswapProvider, IPositionEvaluator positionEvaluator,
        ILogger<PositionSnapshotUpdater> logger)
    {
        _uniswapProvider = uniswapProvider;
        _positionEvaluator = positionEvaluator;
        _logger = logger;
    }

    public async IAsyncEnumerable<UniswapLiquidityPosition> GetUpdatedPositionsAsync(UniswapChainConfiguration chain,
        IReadOnlyCollection<UniswapLiquidityPosition> dbPositions,
        DateOnly snapshotDay,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        if (dbPositions.Count == 0)
        {
            yield break;
        }

        var dbPositionsMap = dbPositions.ToDictionary(x => x.PositionId);

        var positionIds = dbPositionsMap.Keys.ToList();

        var uniswapPositions = await _uniswapProvider.GetPositionsAsync(chain, positionIds);

        foreach (var uniswapPosition in uniswapPositions)
        {
            if (!dbPositionsMap.TryGetValue((ulong)uniswapPosition.PositionId, out var dbPosition))
            {
                continue;
            }

            UniswapLiquidityPosition? updatedPosition = null;

            try
            {
                var pool = await _uniswapProvider.GetPoolAsync(chain, uniswapPosition);

                var valuation =
                    await _positionEvaluator.EvaluatePositionAsync(chain, uniswapPosition, pool, ct);

                var token0 = CryptoTokenStatisticWithFee.From(
                    valuation.PositionTokens.Token0,
                    valuation.PositionFees.Token0);

                var token1 = CryptoTokenStatisticWithFee.From(
                    valuation.PositionTokens.Token1,
                    valuation.PositionFees.Token1);

                dbPosition.AddOrUpdateSnapshot(snapshotDay, valuation.IsInRange, token0, token1);

                updatedPosition = dbPosition;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating position snapshot with position id: {PositionId}",
                    uniswapPosition.PositionId);
            }

            if (updatedPosition != null)
            {
                yield return updatedPosition;
            }

        }
    }
}