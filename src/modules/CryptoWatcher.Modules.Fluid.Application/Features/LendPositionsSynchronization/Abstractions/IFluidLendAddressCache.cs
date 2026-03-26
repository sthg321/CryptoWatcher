using CryptoWatcher.Modules.Fluid.Entities.Supply;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Abstractions;

public interface IFluidLendAddressCache
{
    Task InitializeAsync();

    FluidLendAddress? GetAddress(EvmAddress address);
}