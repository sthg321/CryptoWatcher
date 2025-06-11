using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Entities;
using EFCore.BulkExtensions;

namespace CryptoWatcher.Data;

/// <summary>
/// <see cref="IRepository{TEntity}"/>
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class EfRepository<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity> where TEntity : class
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Dictionary<Type, List<string>> Type2PrimaryKeyFields = new()
    {
        [typeof(PoolPosition)] =
        [
            nameof(PoolPosition.PositionId), nameof(PoolPosition.NetworkName),
            nameof(PoolPosition.Day)
        ],
        [typeof(PoolPositionFee)] =
        [
            nameof(PoolPositionFee.Day), nameof(PoolPositionFee.LiquidityPoolPositionId),
            nameof(PoolPositionFee.NetworkName)
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

        await _dbContext.BulkInsertOrUpdateAsync(entities,
            config => { config.UpdateByProperties = Type2PrimaryKeyFields.GetValueOrDefault(typeof(TEntity)); },
            cancellationToken: ct);
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken ct)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity, ct);

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