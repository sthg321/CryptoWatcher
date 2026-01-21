using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.UniswapV3.OperationReaders;

public class PositionOperationOrchestrator
{
    private readonly IRepository<UniswapLiquidityPosition> _positionRepository;

    public PositionOperationOrchestrator(IRepository<UniswapLiquidityPosition> positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public async Task<PositionOperationInfo?> GetOperationAsync(UniswapChainConfiguration chainConfiguration,
        CancellationToken ct = default)
    {
        return new PositionOperationInfo();
    }
}