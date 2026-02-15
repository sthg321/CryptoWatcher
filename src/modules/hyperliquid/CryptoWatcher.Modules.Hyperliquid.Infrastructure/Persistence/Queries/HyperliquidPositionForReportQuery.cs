using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Hyperliquid.Application.Features.Reports.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Persistence.Queries;

public class HyperliquidPositionForReportQuery : IHyperliquidPositionForReportQuery
{
    private readonly HyperliquidDbContext _dbContext;

    public HyperliquidPositionForReportQuery(HyperliquidDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HyperliquidVaultPosition[]> GetPositionsAsync(IReadOnlyCollection<EvmAddress> wallets,
        DateOnly from, DateOnly to,
        CancellationToken ct = default)
    {
        return await _dbContext.HyperliquidVaultPositions
            .Where(position =>  wallets.Contains(position.WalletAddress))
            .Include(position => position.CashFlows.Where(@event =>
                @event.Date.Date >= from.ToMinDateTime() && @event.Date.Date <= to.ToMaxDateTime()))
            .Include(position => position.Snapshots
                .OrderBy(snapshot => snapshot.Day)
                .Where(snapshot => snapshot.Day >= from && snapshot.Day <= to))
            .ToArrayAsync(ct);
    }
}