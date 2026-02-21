using CryptoWatcher.Abstractions;
using CryptoWatcher.Exceptions;
using CryptoWatcher.Infrastructure.Configuration.Conventions;
using CryptoWatcher.Infrastructure.Configuration.Converters;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Hyperliquid.Entities;
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

    #region Aave

    /// <summary>
    /// Represents the entity set for managing Aave chain configurations in the application's database context.
    /// </summary>
    /// <remarks>
    /// This property allows access to Aave-specific blockchain configuration data, including chain names, RPC details,
    /// and associated smart contract addresses. It is essential to manage and query configuration settings necessary
    /// for interaction with various Aave-supported blockchains within the CryptoWatcher application.
    /// </remarks>
    public DbSet<AaveChainConfiguration> AaveChainConfigurations => Set<AaveChainConfiguration>();

    /// <summary>
    /// Provides access to the set of Aave positions within the application's database context.
    /// </summary>
    /// <remarks>
    /// This property facilitates querying and managing Aave liquidity position data,
    /// which includes details such as the associated network, wallet information, token address,
    /// position type, and relevant timestamps. It serves as the primary interface for interacting
    /// with Aave-specific entities in the database.
    /// </remarks>
    public DbSet<AavePosition> AavePositions => Set<AavePosition>();

    /// <summary>
    /// Represents a collection of historical snapshots for Aave positions in the application's database context.
    /// </summary>
    /// <remarks>
    /// AavePositionSnapshots facilitates querying and managing time-series data related to Aave positions,
    /// such as daily snapshots of token details, position identifiers, and associated metadata.
    /// This property allows for tracking the historical states and changes in Aave positions over time.
    /// </remarks>
    public DbSet<AavePositionSnapshot> AavePositionSnapshots => Set<AavePositionSnapshot>();

    public DbSet<AaveAccountSnapshot> AaveAccountSnapshots => Set<AaveAccountSnapshot>();

    /// <summary>
    /// Represents the daily performance data for Aave positions in the application's database context.
    /// </summary>
    /// <remarks>
    /// This property defines the entity set for managing records of daily performance metrics related to Aave positions.
    /// Each record captures details such as the position's snapshot ID, wallet address, date, network name, and position type.
    /// It is instrumental in tracking and analyzing daily performance trends of Aave positions, enabling enhanced monitoring
    /// and performance reporting within the CryptoWatcher application.
    /// </remarks>
    public DbSet<AavePositionDailyPerformance> AavePositionDailyPerformances => Set<AavePositionDailyPerformance>();

    /// <summary>
    /// Provides access to the set of Aave position events in the application's database context.
    /// </summary>
    /// <remarks>
    /// This property enables querying and managing event data associated with Aave positions.
    /// It is designed to store and retrieve transactional information, such as events specific
    /// to individual position records, including token details and relevant identifiers.
    /// </remarks>
    public DbSet<AavePositionCashFlow> AavePositionCashFlows => Set<AavePositionCashFlow>();

    #endregion

    #region Uniswap

    /// <summary>
    /// Represents the collection of blockchain networks as part of the database context in the CryptoWatcher application.
    /// </summary>
    /// <remarks>
    /// This property defines the entity set for blockchain networks, enabling the application to query and manage
    /// uniswapNetwork-related data stored in the database. Each uniswapNetwork record contains essential configuration
    /// details such as RPC URLs, contract addresses, and associated historical data, which are
    /// critical for interacting with and monitoring blockchain states.
    /// </remarks>
    public DbSet<UniswapChainConfiguration> UniswapChainConfigurations => Set<UniswapChainConfiguration>();

    /// <summary>
    /// Represents the collection of liquidity pool positions as part of the database context in the CryptoWatcher application.
    /// </summary>
    /// <remarks>
    /// This property defines the entity set for managing and querying liquidity pool positions stored in the database.
    /// Each record provides detailed information about token amounts, their corresponding USD values, uniswapNetwork details,
    /// and metadata regarding the liquidity pool position's activity status. It integrates with other entities such as
    /// networks to ensure comprehensive tracking of liquidity metrics.
    /// </remarks>
    public DbSet<UniswapLiquidityPosition> UniswapLiquidityPositions => Set<UniswapLiquidityPosition>();

    /// <summary>
    /// Represents the daily performance metrics of Uniswap liquidity pool positions.
    /// </summary>
    /// <remarks>
    /// This property defines the entity set for managing daily performance data of Uniswap liquidity positions
    /// in the application's database context. Each record includes details such as the associated pool position
    /// ID, network name, and the specific day of the performance metrics. This allows the application to track
    /// and analyze the historical performance of Uniswap positions over time, supporting features like data
    /// visualization, profit calculation, and trend analysis.
    /// </remarks>
    public DbSet<UniswapPositionDailyPerformance> UniswapPositionDailyPerformances =>
        Set<UniswapPositionDailyPerformance>();

    /// <summary>
    /// Represents the collection of Uniswap liquidity position snapshot entities within the application's database context.
    /// </summary>
    /// <remarks>
    /// This property is used to manage and interact with historical snapshots of Uniswap liquidity positions. Each snapshot
    /// provides detailed information regarding a specific liquidity position at a given point in time, including the related
    /// tokens, fee amounts, and valuation metrics. These snapshots facilitate the tracking and analysis of performance
    /// trends, historical data, and liquidity position changes over time.
    /// </remarks>
    public DbSet<UniswapLiquidityPositionSnapshot> UniswapLiquidityPositionSnapshots =>
        Set<UniswapLiquidityPositionSnapshot>();

    /// <summary>
    /// Represents the collection of Uniswap liquidity position cash flow records within the application's database context.
    /// </summary>
    /// <remarks>
    /// This property defines the entity set for managing cash flow data related to Uniswap liquidity positions in the CryptoWatcher application.
    /// Each record tracks the financial events associated with a specific liquidity position, including token transactions and associated fees,
    /// enabling precise monitoring and analysis of position profitability over time.
    /// </remarks>
    public DbSet<UniswapLiquidityPositionCashFlow> UniswapLiquidityPositionCashFlows =>
        Set<UniswapLiquidityPositionCashFlow>();

    #endregion

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