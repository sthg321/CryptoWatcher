using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.Aave.Infrastructure.Persistence;

public class AaveDbContext : BaseDbContext
{
    public AaveDbContext(DbContextOptions<AaveDbContext> options) : base(options)
    {
        
    }
    
     /// <summary>
    /// Represents the entity set for managing Aave chain configurations in the application's database context.
    /// </summary>
    /// <remarks>
    /// This property allows access to Aave-specific blockchain configuration data, including chain names, RPC details,
    /// and associated smart contract addresses. It is essential to manage and query configuration settings necessary
    /// for interaction with various Aave-supported blockchains within the CryptoWatcher application.
    /// </remarks>
    public DbSet<AaveProtocolConfiguration> AaveChainConfigurations => Set<AaveProtocolConfiguration>();

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AaveDbContext).Assembly);
    }
}