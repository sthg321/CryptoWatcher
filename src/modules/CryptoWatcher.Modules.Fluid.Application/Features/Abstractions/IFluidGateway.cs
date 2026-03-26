using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Application.Features.Abstractions;

public interface IFluidGateway
{
    Task<FluidPositionData> GetPositionDataAsync(string network, EvmAddress fTokenAddress, EvmAddress walletAddress);

    Task<FluidFTokenData> GetFTokenDataAsync(string network, EvmAddress fTokenAddress);
}