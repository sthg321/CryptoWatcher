using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Persistence.Repositories;

public class UniswapChainConfigurationRepository : IUniswapChainConfigurationRepository
{
    private readonly UniswapDbContext _context;

    public UniswapChainConfigurationRepository(UniswapDbContext context)
    {
        _context = context;
    }

    public async Task<UniswapChainConfiguration> GetByIdAsync(int chainId, CancellationToken ct = default)
    {
        return await _context.UniswapChainConfigurations.FirstAsync(configuration =>
            configuration.ChainId == chainId, ct);
    }

    public async Task<UniswapChainConfiguration[]> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.UniswapChainConfigurations.ToArrayAsync(ct);
    }
}