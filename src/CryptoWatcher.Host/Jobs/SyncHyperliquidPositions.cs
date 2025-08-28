using CryptoWatcher.Abstractions;
using CryptoWatcher.Data;
using CryptoWatcher.HyperliquidModule;
using CryptoWatcher.HyperliquidModule.Entities;
using Microsoft.EntityFrameworkCore;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Host.Jobs;

public class SyncHyperliquidPositions
{
    private readonly CryptoWatcherDbContext _context;
    private readonly IHyperliquidProvider _hyperliquidProvider;
    private readonly IRepository<HyperliquidVaultPosition> _repository;
    private readonly IRepository<HyperliquidVaultEvent> _eventRepository;
    private readonly IRepository<HyperliquidVaultPositionSnapshot> _snapshotRepository;

    public SyncHyperliquidPositions(CryptoWatcherDbContext context, IHyperliquidProvider hyperliquidProvider,
        IRepository<HyperliquidVaultPosition> repository, IRepository<HyperliquidVaultEvent> eventRepository,
        IRepository<HyperliquidVaultPositionSnapshot> snapshotRepository)
    {
        _context = context;
        _hyperliquidProvider = hyperliquidProvider;
        _repository = repository;
        _eventRepository = eventRepository;
        _snapshotRepository = snapshotRepository;
    }

    [TickerFunction(nameof(SyncHyperliquidPositionsAsync), "0 */1 * * *")]
    public async Task SyncHyperliquidPositionsAsync(CancellationToken ct)
    {
        var wallets = await _context.Wallets.ToArrayAsync(ct);

        var now = DateTime.Now;
        foreach (var wallet in wallets)
        {
            await using var transaction = await _repository.UnitOfWork.BeginTransactionAsync(ct);
            
            var fundingHistory = await _hyperliquidProvider.GetVaultsFundingHistory(wallet, ct);

            var vaultPositions = await _hyperliquidProvider.GetVaultsPositionsEquityAsync(wallet, ct);

            var hyperliquidVaultPositions = fundingHistory.Select(@event => @event.VaultAddress)
                .Distinct()
                .Select(vaultAddress => new HyperliquidVaultPosition
                {
                    WalletAddress = wallet.Address,
                    VaultAddress = vaultAddress,
                    Wallet = wallet
                })
                .ToArray();

            await _repository.BulkMergeAsync(hyperliquidVaultPositions, ct);

            var vaultEvents = fundingHistory.Select(@event =>
                new HyperliquidVaultEvent
                {
                    EventType = @event.EventType,
                    VaultAddress = @event.VaultAddress,
                    Usd = @event.Usd,
                    Date = @event.Date,
                    WalletAddress = wallet.Address,
                }).ToList();

            await _eventRepository.BulkMergeAsync(vaultEvents, ct);

            var vaultPositionSnapshots = vaultPositions.Select(tuple => new HyperliquidVaultPositionSnapshot
            {
                Balance = tuple.Equity,
                Day = DateOnly.FromDateTime(now),
                WalletAddress = wallet.Address,
                VaultAddress = tuple.VaultAddress,
            }).ToList();

            await _snapshotRepository.BulkMergeAsync(vaultPositionSnapshots, ct);

            await _repository.UnitOfWork.SaveChangesAsync(ct);
        }
    }
}