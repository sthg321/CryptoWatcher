using CryptoWatcher.Modules.Infrastructure.Shared.Persistence;
using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Persistence;

public class UniswapDbContext : BaseDbContext
{
    public UniswapDbContext(DbContextOptions<UniswapDbContext> options) : base(options)
    {
    }
    
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
 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("uniswap");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniswapDbContext).Assembly);
    }
}