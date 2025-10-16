using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services;

public interface IHyperliquidPositionsSyncService
{
    /// <summary>
    /// Synchronizes the positions of the given wallet for the specified day.
    /// </summary>
    /// <param name="wallet">The cryptocurrency wallet whose positions are to be synchronized.</param>
    /// <param name="syncDay">The specific day for which positions are being synchronized.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous synchronization operation.</returns>
    Task SyncPositionsAsync(Wallet wallet, DateTime syncDay, CancellationToken ct = default);
}

/// <summary>
/// <see cref="IHyperliquidPositionsSyncService"/>
/// </summary>
public class HyperliquidPositionsSyncService : IHyperliquidPositionsSyncService
{
    private readonly IHyperliquidProvider _hyperliquidProvider;
    private readonly IRepository<HyperliquidVaultPosition> _repository;
    private readonly IRepository<HyperliquidVaultEvent> _eventRepository;
    private readonly IRepository<HyperliquidVaultPositionSnapshot> _snapshotRepository;

    public HyperliquidPositionsSyncService(IHyperliquidProvider hyperliquidProvider,
        IRepository<HyperliquidVaultPosition> repository, IRepository<HyperliquidVaultEvent> eventRepository,
        IRepository<HyperliquidVaultPositionSnapshot> snapshotRepository)
    {
        _hyperliquidProvider = hyperliquidProvider;
        _repository = repository;
        _eventRepository = eventRepository;
        _snapshotRepository = snapshotRepository;
    }

    public async Task SyncPositionsAsync(Wallet wallet, DateTime syncDay, CancellationToken ct = default)
    {
        await _repository.UnitOfWork.BeginTransactionAsync(ct);

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
                Event = @event.Event,
                VaultAddress = @event.VaultAddress,
                Usd = @event.Usd,
                Date = @event.Date,
                WalletAddress = wallet.Address,
            }).ToList();

        await _eventRepository.BulkMergeAsync(vaultEvents, ct);

        var vaultPositionSnapshots = vaultPositions.Select(tuple => new HyperliquidVaultPositionSnapshot
        {
            Balance = tuple.Equity,
            Day = DateOnly.FromDateTime(syncDay),
            WalletAddress = wallet.Address,
            VaultAddress = EvmAddress.Create(tuple.VaultAddress),
        }).ToList();

        await _snapshotRepository.BulkMergeAsync(vaultPositionSnapshots, ct);
        
        await _repository.UnitOfWork.CommitTransactionAsync(ct);
    }
}