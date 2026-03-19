using CryptoWatcher.Abstractions;
using CryptoWatcher.Exceptions;
using CryptoWatcher.Infrastructure.Configuration.Conventions;
using CryptoWatcher.Infrastructure.Configuration.Converters;
using CryptoWatcher.Modules.Merkl.Entities;
using CryptoWatcher.Modules.Morpho.Entities;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmartEnum.EFCore;

namespace CryptoWatcher.Infrastructure;

/// <summary>
/// Represents the Entity Framework Core database context for the CryptoWatcher application.
/// </summary>
/// <remarks>
/// This class provides access to the application's data through DbSet properties for key entities
/// like networks, liquidity pool positions, and pool history. It initializes the database schema
/// and configures entity relationships using Fluent API within the OnModelCreating method.
/// </remarks>
public class CryptoWatcherDbContext(DbContextOptions<CryptoWatcherDbContext> options) : DbContext(options), IUnitOfWork
{
    private const byte EvmAddressLength = 42;
    private const byte TransactionHashLength = 66;

    private IDbContextTransaction? _activeTransaction;

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


    #region Morpho

    public DbSet<MorphoMarketPosition> MorphoMarketPositions => Set<MorphoMarketPosition>();

    public DbSet<MorphoMarketPositionSnapshot> MorphoMarketPositionSnapshots => Set<MorphoMarketPositionSnapshot>();

    #endregion

    #region Merkl

    public DbSet<MerklCampaign> MerklCampaigns => Set<MerklCampaign>();

    public DbSet<MerklCampaignSnapshot> MerklCampaignSnapshots => Set<MerklCampaignSnapshot>();

    public DbSet<MerklCampaignCashFlow> MerklCampaignCashFlows => Set<MerklCampaignCashFlow>();

    #endregion

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ComplexProperties<CryptoToken>();
        configurationBuilder.ComplexProperties<CryptoTokenStatistic>();
        configurationBuilder.ComplexProperties<CryptoTokenStatisticWithFee>();

        configurationBuilder.Properties<EvmAddress>()
            .HaveConversion<EvmAddressConverter>()
            .AreFixedLength()
            .AreUnicode(false)
            .HaveMaxLength(EvmAddressLength);

        configurationBuilder.Properties<EvmAddress?>()
            .HaveConversion<EvmAddressConverter>()
            .AreFixedLength()
            .AreUnicode(false)
            .HaveMaxLength(EvmAddressLength);

        configurationBuilder.Properties<TransactionHash>()
            .HaveConversion<TransactionHashConverter>()
            .AreFixedLength()
            .AreUnicode(false)
            .HaveMaxLength(TransactionHashLength);

        configurationBuilder.Properties<TransactionHash?>()
            .HaveConversion<TransactionHashConverter>()
            .AreFixedLength()
            .AreUnicode(false)
            .HaveMaxLength(TransactionHashLength);

        // Map Uri to string with max length 128 globally
        configurationBuilder.Properties<Uri>()
            .HaveConversion<UriConverter>()
            .HaveMaxLength(128);
        configurationBuilder.Properties<Uri?>()
            .HaveConversion<UriConverter>()
            .HaveMaxLength(128);

        configurationBuilder.Conventions.Add(_ => new TokenInfoSymbolMaxLengthConvention());

        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureSmartEnum();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CryptoWatcherDbContext).Assembly);
    }

    /// <inheritdoc/>
    public async Task BeginTransactionAsync(CancellationToken ct)
    {
        if (_activeTransaction is not null)
        {
            throw new DomainException("Can't start a new transaction while another is active.");
        }

        _activeTransaction = await Database.BeginTransactionAsync(ct);
    }

    /// <inheritdoc/>
    public async Task RollbackTransactionAsync(CancellationToken ct)
    {
        var transaction = _activeTransaction;
        if (transaction is null)
        {
            return;
        }

        _activeTransaction = null;

        try
        {
            await transaction.RollbackAsync(ct);
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }

    public bool HasActiveTransaction => _activeTransaction is not null;

    public async Task CommitTransactionAsync(CancellationToken ct)
    {
        if (_activeTransaction is null)
        {
            throw new DomainException("Transaction is not started");
        }

        try
        {
            await _activeTransaction.CommitAsync(ct);
        }
        finally
        {
            _activeTransaction.Dispose();
            _activeTransaction = null;
        }
    }

    /// <inheritdoc/>
    public new async Task SaveChangesAsync(CancellationToken ct)
    {
        await base.SaveChangesAsync(ct);
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct)
    {
        await using var tr = await Database.BeginTransactionAsync(ct);

        try
        {
            await action(ct);

            await SaveChangesAsync(ct);

            await tr.CommitAsync(ct);
        }
        catch
        {
            await tr.RollbackAsync(ct);
            throw;
        }
    }
}