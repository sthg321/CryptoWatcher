using CryptoWatcher.Abstractions;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule.Abstractions;
using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.UniswapModule.Specifications;

namespace CryptoWatcher.UniswapModule.Services;

/// <summary>
/// <see cref="IPoolHistorySyncRepositoryFacade"/>
/// </summary>
public class PoolHistorySyncRepositoryFacade : IPoolHistorySyncRepositoryFacade
{
    private readonly IRepository<UniswapNetwork> _networkRepository;
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IRepository<PoolPosition> _liquidityPoolPositionRepository;
    private readonly IRepository<PoolPositionSnapshot> _liquidityPoolPositionSnapshotRepository;

    public PoolHistorySyncRepositoryFacade(IRepository<UniswapNetwork> networkRepository,
        IRepository<Wallet> walletRepository, IRepository<PoolPosition> liquidityPoolPositionRepository,
        IRepository<PoolPositionSnapshot> liquidityPoolPositionSnapshotRepository)
    {
        _networkRepository = networkRepository;
        _walletRepository = walletRepository;
        _liquidityPoolPositionRepository = liquidityPoolPositionRepository;
        _liquidityPoolPositionSnapshotRepository = liquidityPoolPositionSnapshotRepository;
    }

    public async Task<List<UniswapNetwork>> GetNetworksAsync(CancellationToken ct = default)
    {
        return await _networkRepository.ListAsync(ct);
    }

    public async Task<List<Wallet>> GetWalletsAsync(CancellationToken ct = default)
    {
        return await _walletRepository.ListAsync(ct);
    }

    public async Task<List<PoolPosition>> GetLiquidityPoolPositionsAsync(UniswapNetwork uniswapNetwork, Wallet wallet,
        CancellationToken ct = default)
    {
        return await _liquidityPoolPositionRepository.ListAsync(
            new GetPositionsByWalletAndNetworkSpecification(uniswapNetwork, wallet), ct);
    }

    public async Task MergePoolPositionsAsync(IList<PoolPosition> positions,
        IList<PoolPositionSnapshot> snapshots,
        CancellationToken ct = default)
    {
        await using var tr = await _liquidityPoolPositionRepository.UnitOfWork.BeginTransactionAsync(ct);
        try
        {
            await _liquidityPoolPositionRepository.BulkMergeAsync(positions, ct);

            await _liquidityPoolPositionSnapshotRepository.BulkMergeAsync(snapshots, ct);

            await _liquidityPoolPositionRepository.UnitOfWork.SaveChangesAsync(ct);
        }
        catch
        {
            await _liquidityPoolPositionRepository.UnitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}