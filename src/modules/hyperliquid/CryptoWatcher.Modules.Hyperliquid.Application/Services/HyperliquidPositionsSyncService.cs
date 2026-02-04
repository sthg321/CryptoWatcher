using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Services.PositionUpdates;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Modules.Hyperliquid.Specifications;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services;

/// <summary>
/// <see cref="IHyperliquidPositionsSyncService"/>
/// </summary>
public class HyperliquidPositionsSyncService : IHyperliquidPositionsSyncService
{
    private readonly IHyperliquidGateway _hyperliquidGateway;
    private readonly IRepository<HyperliquidVaultPosition> _repository;
    private readonly TimeProvider _timeProvider;
    private readonly IHyperliquidSyncRepoFacade _facade;

    public HyperliquidPositionsSyncService(IHyperliquidGateway hyperliquidGateway,
        IRepository<HyperliquidVaultPosition> repository,
        TimeProvider timeProvider, IHyperliquidSyncRepoFacade facade)
    {
        _hyperliquidGateway = hyperliquidGateway;
        _repository = repository;

        _timeProvider = timeProvider;
        _facade = facade;
    }

    public async Task SyncPositionsAsync(Wallet wallet, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        var existingPositionMap =
            (await _repository.ListAsync(
                new HyperliquidPositionsWithSnapshotsAndCashFlowByWallet(wallet.Address, from, to),
                ct))
            .ToDictionary(position => position.VaultAddress);

        var hyperliquidVaultPositions = await _hyperliquidGateway.GetVaultsPositionsEquityAsync(wallet.Address, ct);

        var cashFlowHistory =
            (await _hyperliquidGateway.GetVaultUpdatesAsync(wallet.Address, from.ToMinDateTime(), to.ToMaxDateTime(),
                ct))
            .GroupBy(@event => @event.VaultAddress)
            .ToDictionary(events => events.Key, events => events.OrderBy(@event => @event.Timestamp).ToArray());

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var nowDay = DateOnly.FromDateTime(now);

        await _repository.UnitOfWork.BeginTransactionAsync(ct);

        var result = new List<HyperliquidVaultPosition>();

        foreach (var vault in hyperliquidVaultPositions)
        {
            try
            {
                var isPositionExist = existingPositionMap.TryGetValue(vault.Address, out var vaultPosition);
                if (!isPositionExist)
                {
                    vaultPosition = new HyperliquidVaultPosition(vault.Balance, now, vault.Address, wallet.Address);
                }
                
                if (cashFlowHistory.TryGetValue(vault.Address, out var cashFlowEvents))
                {
                    foreach (var cashFlowEvent in cashFlowEvents)
                    {
                        vaultPosition!.AddCashFlowIfNotExists(cashFlowEvent.Amount,
                            cashFlowEvent is DepositUpdate ? CashFlowEvent.Deposit : CashFlowEvent.Withdrawal,
                            cashFlowEvent.Timestamp);
                    }
                }

                vaultPosition!.AddOrUpdateSnapshot(vault.Balance, nowDay);
 
                result.Add(vaultPosition);
            }
            catch
            {
                await _repository.UnitOfWork.RollbackTransactionAsync(ct);
                throw;
            }
        }

        await _facade.SavePositionWithGraphAsync(result, ct);
        await _repository.UnitOfWork.CommitTransactionAsync(ct);
    }
}