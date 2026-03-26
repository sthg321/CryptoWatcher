using CryptoWatcher.Modules.Fluid.Abstractions;
using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Abstractions;
using CryptoWatcher.Modules.Fluid.Entities.Supply;
using CryptoWatcher.ValueObjects;
using Microsoft.Extensions.Caching.Memory;

namespace CryptoWatcher.Modules.Fluid.Application.Features;

public class FluidLendAddressCache : IFluidLendAddressCache
{
    private const string CacheKeyTemplate = "fluid:lend:address:{0}";

    private readonly IMemoryCache _cache;
    private readonly IFluidLendAddressRepository _addressRepository;

    public FluidLendAddressCache(IMemoryCache cache, IFluidLendAddressRepository addressRepository)
    {
        _cache = cache;
        _addressRepository = addressRepository;
    }

    public async Task InitializeAsync()
    {
        var addresses = await _addressRepository.GetAllAsync();

        foreach (var fluidLendAddress in addresses)
        {
            var key = string.Format(CacheKeyTemplate, fluidLendAddress.Address.Value);
            _cache.Set(key, fluidLendAddress);
        }
    }

    public FluidLendAddress? GetAddress(EvmAddress address)
    {
        var key = string.Format(CacheKeyTemplate, address.Value);
        return _cache.Get<FluidLendAddress>(key);
    }
}