using CryptoWatcher.Modules.Fluid.Abstractions;
using CryptoWatcher.Modules.Fluid.Entities.Supply;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Persistence.Repositories;

public class FluidLendAddressRepository : IFluidLendAddressRepository
{
    private readonly FluidDbContext _dbContext;

    public FluidLendAddressRepository(FluidDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FluidLendAddress[]> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbContext.FluidLendAddresses.ToArrayAsync(ct);
    }
}