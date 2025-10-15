using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Infrastructure;

/// <summary>
/// <see cref="IRepository{TEntity}"/>
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class EfRepository<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity> where TEntity : class
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Dictionary<Type, List<string>> Type2PrimaryKeyFields = new()
    {
        [typeof(Wallet)] =
        [
            nameof(Wallet.Address)
        ],
        [typeof(UniswapLiquidityPosition)] =
        [
            nameof(UniswapLiquidityPosition.PositionId), nameof(UniswapLiquidityPosition.NetworkName)
        ],
        [typeof(UniswapLiquidityPositionSnapshot)] =
        [
            nameof(UniswapLiquidityPositionSnapshot.Day), nameof(UniswapLiquidityPositionSnapshot.PoolPositionId),
            nameof(UniswapLiquidityPositionSnapshot.NetworkName)
        ],
        [typeof(HyperliquidVaultPosition)] =
        [
            nameof(HyperliquidVaultPosition.VaultAddress), nameof(HyperliquidVaultPosition.WalletAddress)
        ],
        [typeof(HyperliquidVaultEvent)] =
        [
            nameof(HyperliquidVaultEvent.VaultAddress), nameof(HyperliquidVaultEvent.WalletAddress),
            nameof(HyperliquidVaultEvent.Date)
        ],
        [typeof(HyperliquidVaultPositionSnapshot)] =
        [
            nameof(HyperliquidVaultPositionSnapshot.VaultAddress),
            nameof(HyperliquidVaultPositionSnapshot.WalletAddress),
            nameof(HyperliquidVaultPositionSnapshot.Day)
        ],
    };

    private readonly CryptoWatcherDbContext _dbContext;

    public EfRepository(CryptoWatcherDbContext dbContext, IUnitOfWork unitOfWork) :
        base(dbContext)
    {
        _dbContext = dbContext;
        UnitOfWork = unitOfWork;
    }

    public EfRepository(CryptoWatcherDbContext dbContext, ISpecificationEvaluator specificationEvaluator,
        IUnitOfWork unitOfWork) : base(dbContext, specificationEvaluator)
    {
        _dbContext = dbContext;
        UnitOfWork = unitOfWork;
    }
 
    public async Task BulkMergeAsync(IList<TEntity> entities, CancellationToken ct)
    {
        if (entities.Count == 0)
        {
            return;
        }
        
        await _dbContext.BulkMergeAsync(entities, operation =>
        {
            operation.ColumnPrimaryKeyNames = Type2PrimaryKeyFields.GetValueOrDefault(typeof(TEntity));
        }, ct);
    
    }

    public TEntity Insert(TEntity entity)
    {
        _dbContext.Set<TEntity>().Add(entity);

        return entity;
    }

    public void Update(TEntity entity)
    {
        _dbContext.Set<TEntity>().Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
    }

    public IUnitOfWork UnitOfWork { get; }
}