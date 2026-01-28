using CryptoWatcher.Abstractions;
using CryptoWatcher.Application;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsSnapshotSynchronization.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsSnapshotSynchronization;

public class PositionSnapshotSynchronizationJob :
    BaseOnChainSynchronizationJob<UniswapChainConfiguration, UniswapPositionsContext>, IPositionPriceSynchronizationJob
{
    private const int ChunkSize = 500;

    private readonly IRepository<UniswapLiquidityPosition> _positionsRepository;
    private readonly IPositionSnapshotUpdater _positionSnapshotUpdater;
    private readonly ILogger<PositionSnapshotSynchronizationJob> _logger;

    public PositionSnapshotSynchronizationJob(IRepository<Wallet> walletRepository,
        IRepository<UniswapChainConfiguration> chainRepository,
        IRepository<UniswapLiquidityPosition> positionsRepository,
        IPositionSnapshotUpdater positionSnapshotUpdater,
        ILogger<PositionSnapshotSynchronizationJob> logger) : base(walletRepository, chainRepository, logger)
    {
        _positionsRepository = positionsRepository;
        _positionSnapshotUpdater = positionSnapshotUpdater;
        _logger = logger;
    }

    protected override async Task<UniswapPositionsContext> CreateContextAsync(CancellationToken ct)
    {
        var positions = await _positionsRepository.ListAsync(ct);

        return new UniswapPositionsContext(positions);
    }

    protected override async Task SynchronizeWalletOnChainAsync(UniswapChainConfiguration chain, Wallet wallet,
        UniswapPositionsContext context,
        CancellationToken ct)
    {
        using var _ = _logger.BeginScope("Uniswap protocol version: {UniswapProtocolVersion}", chain.ProtocolVersion);

        var existedChainPositions = context.GetPositionsForChain(chain);

        if (existedChainPositions.Length == 0)
        {
            _logger.LogInformation("No positions found for wallet");
            return;
        }

        var day = DateOnly.FromDateTime(DateTime.UtcNow);

        var updatedPositions = _positionSnapshotUpdater.GetUpdatedPositionsAsync(chain, existedChainPositions, day, ct);

        await foreach (var chunk in updatedPositions.Chunk(ChunkSize).WithCancellation(ct))
        {
            await _positionsRepository.BulkMergeAsync(chunk, ct);
        }
    }
}