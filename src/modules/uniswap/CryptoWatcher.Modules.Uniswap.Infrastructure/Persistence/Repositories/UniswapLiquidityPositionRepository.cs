using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Persistence.Repositories;

public class UniswapLiquidityPositionRepository : IUniswapLiquidityPositionRepository
{
    private readonly UniswapDbContext _context;

    public UniswapLiquidityPositionRepository(UniswapDbContext context)
    {
        _context = context;
    }

    public async Task<UniswapLiquidityPosition[]> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.UniswapLiquidityPositions.ToArrayAsync(ct);
    }

    public async Task<IReadOnlyCollection<UniswapLiquidityPosition>> GetActiveAsync(UniswapChainConfiguration chain,
        ulong[] positionIds, CancellationToken ct = default)
    {
        return await _context.UniswapLiquidityPositions.Where(position =>
                ((IEnumerable<ulong>)positionIds).Contains(position.PositionId) &&
                position.ProtocolVersion ==
                chain.ProtocolVersion &&
                position.NetworkName == chain.Name)
            .Include(position => position.Snapshots)
            .Include(position => position.CashFlows)
            .ToArrayAsync(ct);
    }

    public async Task<UniswapLiquidityPosition[]> GetForReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly from,
        DateOnly to, CancellationToken ct = default)
    {
        var walletAddresses = wallets.Select(x => x.Address.Value).ToArray();
        
        return await _context.UniswapLiquidityPositions
            .Include(position => position.Wallet)
            .Include(poolPosition => poolPosition.Snapshots
                .Where(snapshot => snapshot.Day >= from && snapshot.Day <= to)
                .OrderBy(snapshot => snapshot.Day)
            )
            .Include(poolPosition => poolPosition.CashFlows)
            .Where(position => ((IEnumerable<string>)walletAddresses).Contains(position.WalletAddress))
            .ToArrayAsync(ct);
    }

    public async Task SaveAsync(UniswapLiquidityPosition[] positions, CancellationToken ct = default)
    {
        await _context.BulkMergeAsync(positions, operation => { }, ct);
    }
}