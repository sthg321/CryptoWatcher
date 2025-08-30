using CryptoWatcher.Abstractions;
using CryptoWatcher.HyperliquidModule.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CryptoWatcher.Infrastructure;

/// <summary>
/// Represents the Entity Framework Core database context for the CryptoWatcher application.
/// </summary>
/// <remarks>
/// This class provides access to the application's data through DbSet properties for key entities
/// like networks, liquidity pool positions, and pool history. It initializes the database schema
/// and configures entity relationships using Fluent API within the OnModelCreating method.
/// </remarks>
public class CryptoWatcherDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    private IDbContextTransaction? _activeTransaction;

    /// <summary>
    /// Represents the collection of blockchain networks as part of the database context in the CryptoWatcher application.
    /// </summary>
    /// <remarks>
    /// This property defines the entity set for blockchain networks, enabling the application to query and manage
    /// uniswapNetwork-related data stored in the database. Each uniswapNetwork record contains essential configuration
    /// details such as RPC URLs, contract addresses, and associated historical data, which are
    /// critical for interacting with and monitoring blockchain states.
    /// </remarks>
    public DbSet<UniswapNetwork> Networks => Set<UniswapNetwork>();

    /// <summary>
    /// Represents the collection of user wallet entities within the application's database context.
    /// </summary>
    /// <remarks>
    /// This property defines the entity set for managing wallets in the CryptoWatcher application. Each wallet record
    /// includes details such as unique identifiers and blockchain addresses, allowing the application to track and
    /// manage user wallet information effectively. This is essential for associating users with addresses,
    /// performing transactions, and retrieving relevant blockchain data.
    /// </remarks>
    public DbSet<Wallet> Wallets => Set<Wallet>();

    /// <summary>
    /// Represents the collection of liquidity pool positions as part of the database context in the CryptoWatcher application.
    /// </summary>
    /// <remarks>
    /// This property defines the entity set for managing and querying liquidity pool positions stored in the database.
    /// Each record provides detailed information about token amounts, their corresponding USD values, uniswapNetwork details,
    /// and metadata regarding the liquidity pool position's activity status. It integrates with other entities such as
    /// networks to ensure comprehensive tracking of liquidity metrics.
    /// </remarks>
    public DbSet<PoolPosition> PoolPositions => Set<PoolPosition>();

    public DbSet<PoolPositionSnapshot> PoolPositionSnapshots => Set<PoolPositionSnapshot>();

    public DbSet<HyperliquidVaultPosition> HyperliquidVaultPositions => Set<HyperliquidVaultPosition>();

    public DbSet<HyperliquidVaultEvent> HyperliquidVaultEvents => Set<HyperliquidVaultEvent>();
    
    public DbSet<HyperliquidVaultPositionSnapshot> HyperliquidVaultPositionSnapshots =>
        Set<HyperliquidVaultPositionSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CryptoWatcherDbContext).Assembly);
    }

    /// <inheritdoc/>
    public async Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken ct)
    {
        return _activeTransaction = await Database.BeginTransactionAsync(ct);
    }

    /// <inheritdoc/>
    public async Task RollbackTransactionAsync(CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_activeTransaction);
        await _activeTransaction.RollbackAsync(ct);
    }

    /// <inheritdoc/>
    public new async Task SaveChangesAsync(CancellationToken ct)
    {
        await base.SaveChangesAsync(ct);

        if (_activeTransaction is not null)
        {
            await _activeTransaction.CommitAsync(ct);
        }
    }
}