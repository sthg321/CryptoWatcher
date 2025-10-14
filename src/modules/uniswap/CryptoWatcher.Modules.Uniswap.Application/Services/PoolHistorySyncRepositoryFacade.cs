using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Specifications;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class PoolHistorySyncRepositoryFacade : IPoolHistorySyncRepositoryFacade
{
    private readonly IRepository<UniswapLiquidityPosition> _liquidityPoolPositionRepository;
    private readonly IRepository<UniswapLiquidityPositionSnapshot> _liquidityPoolPositionSnapshotRepository;

    public PoolHistorySyncRepositoryFacade(IRepository<UniswapLiquidityPosition> liquidityPoolPositionRepository,
        IRepository<UniswapLiquidityPositionSnapshot> liquidityPoolPositionSnapshotRepository)
    {
        _liquidityPoolPositionRepository = liquidityPoolPositionRepository;
        _liquidityPoolPositionSnapshotRepository = liquidityPoolPositionSnapshotRepository;
    }

    public async Task<List<UniswapLiquidityPosition>> GetLiquidityPoolPositionsAsync(
        UniswapChainConfiguration chainConfiguration,
        Wallet wallet,
        CancellationToken ct = default)
    {
        return await _liquidityPoolPositionRepository.ListAsync(
            new GetPositionsByWalletAndNetworkSpecification(chainConfiguration, wallet), ct);
    }

    public async Task MergePoolPositionsAsync(IList<UniswapLiquidityPosition> positions,
        IList<UniswapLiquidityPositionSnapshot> snapshots,
        CancellationToken ct = default)
    {
        await _liquidityPoolPositionRepository.UnitOfWork.BeginTransactionAsync(ct);
        try
        {
            await _liquidityPoolPositionRepository.BulkMergeAsync(positions, ct);

            await _liquidityPoolPositionSnapshotRepository.BulkMergeAsync(snapshots, ct);

            await _liquidityPoolPositionRepository.UnitOfWork.SaveChangesAsync(ct);

            await _liquidityPoolPositionRepository.UnitOfWork.CommitTransactionAsync(ct);
        }
        catch
        {
            await _liquidityPoolPositionRepository.UnitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}