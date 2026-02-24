using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.Aave.Infrastructure.Persistence.Repositories;

public class AavePositionRepository : IAavePositionRepository
{
    private readonly AaveDbContext _dbContext;

    public AavePositionRepository(AaveDbContext dbContext)
    {
        _dbContext = dbContext;
    } 

    public async Task<IReadOnlyList<AavePosition>> GetActiveForWalletAsync(string network, EvmAddress wallet, DateOnly day, CancellationToken ct)
    {
        return await _dbContext.AavePositions
            .Where(position => wallet == position.WalletAddress && position.Network == network)
            .Include(position => position.Snapshots)
            .Include(position => position.CashFlows)
            .ToArrayAsync(ct);
    }

    public void Add(AavePosition position)
    {
        _dbContext.AavePositions.Add(position);
    }

    public void Update(AavePosition position)
    {
        _dbContext.AavePositions.Update(position);
    }

    public async Task SaveAsync(CancellationToken ct)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}