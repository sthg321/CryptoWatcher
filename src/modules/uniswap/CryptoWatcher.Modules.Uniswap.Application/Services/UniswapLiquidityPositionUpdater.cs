using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapLiquidityPositionUpdater
{
    private readonly IRepository<UniswapLiquidityPosition> _positionsRepository;
    private readonly ILiquidityPositionEventReducer _positionEventReducer;

    public UniswapLiquidityPositionUpdater(IRepository<UniswapLiquidityPosition> positionsRepository,
        ILiquidityPositionEventReducer positionEventReducer)
    {
        _positionsRepository = positionsRepository;
        _positionEventReducer = positionEventReducer;
    }

    public async Task<IEnumerable<UniswapLiquidityPosition>> SaveUpdatedPositionsAsync(
        UniswapChainConfiguration chainConfiguration,
        EvmAddress walletAddress,
        UniswapEvent[] uniswapEventBatch,
        CancellationToken ct = default)
    {
        var positionIds = uniswapEventBatch.Select(@event => @event.Operation.PositionId).Distinct().ToArray();

        var currentPositions = await _positionsRepository.ListAsync(new LiquidityPositionByIds(positionIds), ct);

        return await _positionEventReducer.ApplyEventsAsync(chainConfiguration, walletAddress,
            uniswapEventBatch, currentPositions, ct);
    }
}