using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Modules.Merkl.Entities;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using Z.BulkOperations;

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
        [typeof(AaveAccountSnapshot)] =
        [
            nameof(AaveAccountSnapshot.NetworkName), nameof(AaveAccountSnapshot.WalletAddress),
            nameof(AaveAccountSnapshot.Day)
        ],
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
        [typeof(UniswapLiquidityPositionCashFlow)] =
        [
            nameof(UniswapLiquidityPositionCashFlow.PositionId), nameof(UniswapLiquidityPositionCashFlow.NetworkName),
            nameof(UniswapLiquidityPositionCashFlow.TransactionHash)
        ],
        [typeof(HyperliquidVaultPosition)] =
        [
            nameof(HyperliquidVaultPosition.VaultAddress), nameof(HyperliquidVaultPosition.WalletAddress)
        ],
        [typeof(HyperliquidPositionCashFlow)] =
        [
            nameof(HyperliquidPositionCashFlow.VaultAddress), nameof(HyperliquidPositionCashFlow.WalletAddress),
            nameof(HyperliquidPositionCashFlow.Date)
        ],
        [typeof(HyperliquidVaultPositionSnapshot)] =
        [
            nameof(HyperliquidVaultPositionSnapshot.VaultAddress),
            nameof(HyperliquidVaultPositionSnapshot.WalletAddress),
            nameof(HyperliquidVaultPositionSnapshot.Day)
        ],
        [typeof(MerklCampaignCashFlow)] =
        [
            nameof(MerklCampaignCashFlow.Id),
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

        if (entities.First() is HyperliquidVaultPosition)
        {
            List<AuditEntry> auditEntries = new List<AuditEntry>();
            await _dbContext.BulkMergeAsync(entities.Cast<HyperliquidVaultPosition>(), operation =>
            {
                operation.IncludeGraph = true;
                operation.IncludeGraphOperationBuilder = bulkOperation =>
                {
                    bulkOperation.UseAudit = true;
                    bulkOperation.AuditMode = AuditModeType.IncludeAll;
                    bulkOperation.AuditEntries = auditEntries;
                    bulkOperation.Log = Console.Write;
                    switch (bulkOperation)
                    {
                        case BulkOperation<Wallet> walletOperation:
                            walletOperation.ColumnPrimaryKeyExpression = wallet => wallet.Address;
                            break;
                        case BulkOperation<HyperliquidVaultPosition> positionOperation:
                            positionOperation.ColumnPrimaryKeyExpression = position =>
                                new { position.VaultAddress, position.WalletAddress };
                            break;

                        case BulkOperation<HyperliquidVaultPositionSnapshot> positionOperation:
                            positionOperation.ColumnPrimaryKeyExpression = position =>
                                new { position.VaultAddress, position.WalletAddress, position.Day };
                            break;

                        case BulkOperation<HyperliquidPositionCashFlow> positionOperation:
                            positionOperation.ColumnPrimaryKeyExpression = @event =>
                                new
                                {
                                    @event.VaultAddress, @event.WalletAddress, @event.Date
                                };
                            break;
                        case BulkOperation<HyperliquidVaultPeriod> positionOperation:
                            positionOperation.ColumnPrimaryKeyExpression = @event => @event.Id;
                            break;
                    }
                };
            }, ct);
            return;
        }

        if (entities.First() is UniswapLiquidityPosition)
        {
            await _dbContext.BulkMergeAsync(entities, operation =>
            {
                operation.IncludeGraph = true;
                operation.IncludeGraphOperationBuilder = bulkOperation =>
                {
                    switch (bulkOperation)  
                    {
                        case BulkOperation<Wallet> walletOperation:
                            walletOperation.IsReadOnly = true;
                            break;
                        case BulkOperation<UniswapLiquidityPosition> positionOperation:
                            positionOperation.ColumnPrimaryKeyExpression = position =>
                                new { position.PositionId, position.NetworkName };
                            break;

                        case BulkOperation<UniswapLiquidityPositionSnapshot> positionOperation:
                            positionOperation.ColumnPrimaryKeyExpression = position =>
                                new { position.PoolPositionId, position.NetworkName, position.Day };
                            break;

                        case BulkOperation<UniswapLiquidityPositionCashFlow> positionOperation:
                            positionOperation.ColumnPrimaryKeyExpression = position =>
                                new
                                {
                                    position.PositionId, position.NetworkName, position.TransactionHash, position.Event
                                };
                            break;
                    }
                };
            }, ct);
            return;
        }

        await _dbContext.BulkMergeAsync(entities,
            operation =>
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