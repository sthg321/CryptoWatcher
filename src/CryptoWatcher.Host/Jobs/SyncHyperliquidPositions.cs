using CryptoWatcher.Abstractions;
using CryptoWatcher.Data;
using CryptoWatcher.Entities.Hyperliquid;
using CryptoWatcher.Integrations;
using Microsoft.EntityFrameworkCore;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Host.Jobs;

public class SyncHyperliquidPositions
{
    private readonly CryptoWatcherDbContext _context;
    private readonly IHyperliquidProvider _hyperliquidProvider;
    private readonly IRepository<HyperliquidVaultPosition> _repository;

    public SyncHyperliquidPositions(CryptoWatcherDbContext context, IHyperliquidProvider hyperliquidProvider,
        IRepository<HyperliquidVaultPosition> repository)
    {
        _context = context;
        _hyperliquidProvider = hyperliquidProvider;
        _repository = repository;
    }

    [TickerFunction(nameof(SyncHyperliquidPositionsAsync), "0 */1 * * *")]
    public async Task SyncHyperliquidPositionsAsync(CancellationToken ct)
    {
        var wallets = await _context.Wallets.ToArrayAsync(ct);

        var now = DateTime.Now;
        foreach (var wallet in wallets)
        {
            var fundingHistory = await _hyperliquidProvider.GetVaultsFundingHistory(wallet, ct);

            var vaultPositions = await _hyperliquidProvider.GetVaultsPositionsEquityAsync(wallet, ct);

            var hyperliquidVaultPositions = fundingHistory.Select(@event => @event.VaultAddress)
                .Distinct()
                .Select(vaultAddress => new HyperliquidVaultPosition
                {
                    WalletAddress = wallet.Address,
                    VaultAddress = vaultAddress,
                    VaultEvents = fundingHistory.Where(@event => @event.VaultAddress == vaultAddress).Select(@event =>
                        new HyperliquidVaultEvent
                        {
                            EventType = @event.EventType,
                            VaultAddress = @event.VaultAddress,
                            Usd = @event.Usd,
                            Date = @event.Date,
                            WalletAddress = wallet.Address,
                        }).ToList(),
                    PositionSnapshots = vaultPositions.Where(tuple => tuple.VaultAddress == vaultAddress)
                        .Select(tuple => new HyperliquidVaultPositionSnapshot
                        {
                            Balance = tuple.Equity,
                            Day = DateOnly.FromDateTime(now),
                            WalletAddress = wallet.Address,
                            VaultAddress = tuple.VaultAddress,
                        })
                        .ToList()
                })
                .ToArray();

            await _repository.BulkMergeWithGraphAsync(hyperliquidVaultPositions, ct);
        }
    }
}