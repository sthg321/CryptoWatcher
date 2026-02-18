using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Morpho.Entities;
using CryptoWatcher.Modules.Morpho.Specifications;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Infrastructure.Services;

public class LendingService
{
    private readonly IRepository<AaveAccountSnapshot> _repository;
    private readonly IRepository<MorphoMarketPosition> _morphoRepository;
    
    public LendingService(IRepository<AaveAccountSnapshot> repository, IRepository<MorphoMarketPosition> morphoRepository)
    {
        _repository = repository;
        _morphoRepository = morphoRepository;
    }

    public async Task Test(EvmAddress walletAddress,DateOnly day)
    {
        var aave = await _repository.ListAsync();

        var morpho = await _morphoRepository.ListAsync(new MorphoMarketActivePositions(walletAddress, day, day));
    }
}