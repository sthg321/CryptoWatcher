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
    
    public DbSet<FluidSupplyPosition> FluidSupplyPositions => Set<FluidSupplyPosition>();
    
    public DbSet<FluidSupplyPositionCashFlow> FluidSupplyPositionCashFlows => Set<FluidSupplyPositionCashFlow>();
    
    public DbSet<FluidSupplyPositionPeriod> FluidSupplyPositionPeriods => Set<FluidSupplyPositionPeriod>();
    
    public DbSet<FluidSupplyPositionSnapshot> FluidSupplyPositionSnapshots => Set<FluidSupplyPositionSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("fluid");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FluidDbContext).Assembly);
    }
}