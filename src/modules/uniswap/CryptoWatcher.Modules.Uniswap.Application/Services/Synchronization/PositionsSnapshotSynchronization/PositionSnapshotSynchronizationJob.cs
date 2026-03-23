using CryptoWatcher.Abstractions;
using CryptoWatcher.Application;
using CryptoWatcher.Modules.Uniswap.Abstractions;
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

    private readonly IPositionSnapshotUpdater _positionSnapshotUpdater;
    private readonly IUniswapLiquidityPositionRepository _positionsRepository;
    private readonly IUniswapChainConfigurationRepository _configurationRepository;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<PositionSnapshotSynchronizationJob> _logger;

    public PositionSnapshotSynchronizationJob(IRepository<Wallet> walletRepository,  
        IPositionSnapshotUpdater positionSnapshotUpdater, IUniswapLiquidityPositionRepository positionsRepository,
        IUniswapChainConfigurationRepository configurationRepository, TimeProvider timeProvider,
        ILogger<PositionSnapshotSynchronizationJob> logger) : base(walletRepository, logger)
    {
        _positionSnapshotUpdater = positionSnapshotUpdater;
        _positionsRepository = positionsRepository;
        _configurationRepository = configurationRepository;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    protected override async Task<UniswapPositionsContext> CreateContextAsync(CancellationToken ct)
    {
        var positions = await _positionsRepository.GetAllAsync(ct);

        return new UniswapPositionsContext(positions);
    }

    protected override async Task<UniswapChainConfiguration[]> GetChainConfiguration(CancellationToken ct)
    {
        return await _configurationRepository.GetAllAsync(ct);
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

        var day = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);

        var updatedPositions = _positionSnapshotUpdater.GetUpdatedPositionsAsync(chain, existedChainPositions, day, ct);

        await foreach (var chunk in updatedPositions.Chunk(ChunkSize).WithCancellation(ct))
        {
            await _positionsRepository.SaveAsync(chunk, ct);
        }
    }
}