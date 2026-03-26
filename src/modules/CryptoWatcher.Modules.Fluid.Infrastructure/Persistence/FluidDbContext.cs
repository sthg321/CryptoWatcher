using CryptoWatcher.Modules.Fluid.Entities.Supply;
using CryptoWatcher.Modules.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Persistence;

public class FluidDbContext : BaseDbContext
{
    public FluidDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<FluidLendAddress> FluidLendAddresses => Set<FluidLendAddress>();
    
    public DbSet<FluidLendPosition> FluidSupplyPositions => Set<FluidLendPosition>();
    
    public DbSet<FluidLendPositionCashFlow> FluidSupplyPositionCashFlows => Set<FluidLendPositionCashFlow>();
    
    public DbSet<FluidLendPositionPeriod> FluidSupplyPositionPeriods => Set<FluidLendPositionPeriod>();
    
    public DbSet<FluidLendPositionSnapshot> FluidSupplyPositionSnapshots => Set<FluidLendPositionSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("fluid");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FluidDbContext).Assembly);
    }
}