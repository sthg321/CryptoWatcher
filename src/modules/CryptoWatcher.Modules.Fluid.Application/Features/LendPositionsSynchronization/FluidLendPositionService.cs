using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Fluid.Abstractions;

namespace CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization;

public class FluidLendPositionService
{
    private readonly IFluidLendPositionRepository _positionRepository;

    public FluidLendPositionService(IFluidLendPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public async Task GetOrCreateAsync(BlockchainTransaction transaction)
    {
        
    }
}