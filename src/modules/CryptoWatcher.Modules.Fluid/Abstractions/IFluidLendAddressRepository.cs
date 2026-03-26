using CryptoWatcher.Modules.Fluid.Entities.Supply;

namespace CryptoWatcher.Modules.Fluid.Abstractions;

public interface IFluidLendAddressRepository
{
    Task<FluidLendAddress[]> GetAllAsync(CancellationToken ct = default);
}