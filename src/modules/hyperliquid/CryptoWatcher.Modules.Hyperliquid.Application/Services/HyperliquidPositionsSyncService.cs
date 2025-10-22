using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Modules.Hyperliquid.Specifications;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services;

/// <summary>
/// <see cref="IHyperliquidPositionsSyncService"/>
/// </summary>
public class HyperliquidPositionsSyncService : IHyperliquidPositionsSyncService
{
    private readonly IHyperliquidProvider _hyperliquidProvider;
    private readonly IRepository<HyperliquidVaultPosition> _repository;
    private readonly TimeProvider _timeProvider;

    public HyperliquidPositionsSyncService(IHyperliquidProvider hyperliquidProvider,
        IRepository<HyperliquidVaultPosition> repository,
        TimeProvider timeProvider)
    {
        _hyperliquidProvider = hyperliquidProvider;
        _repository = repository;

        _timeProvider = timeProvider;
    }

    public async Task SyncPositionsAsync(Wallet wallet, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        var existingPositionMap =
            (await _repository.ListAsync(new HyperliquidPositionsWithSnapshotsAndCashFlowByWallet(wallet, from, to),
                ct))
            .ToDictionary(position => position.VaultAddress);

        var hyperliquidVaultPositions = await _hyperliquidProvider.GetVaultsPositionsEquityAsync(wallet, ct);

        var cashFlowHistory = (await _hyperliquidProvider.GetCashFlowEventsAsync(wallet, from, to, ct))
            .GroupBy(@event => @event.VaultAddress)
            .ToDictionary(events => events.Key, events => events.OrderBy(@event => @event.Date).ToArray());

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var nowDay = DateOnly.FromDateTime(now);

        await _repository.UnitOfWork.BeginTransactionAsync(ct);

        var result = new List<HyperliquidVaultPosition>(hyperliquidVaultPositions.Count);

        foreach (var vault in hyperliquidVaultPositions)
        {
            try
            {
                var isPositionExist = existingPositionMap.TryGetValue(vault.Address, out var vaultPosition);
                if (!isPositionExist)
                {
                    vaultPosition = new HyperliquidVaultPosition
                    {
                        InitialBalance = vault.Balance,
                        WalletAddress = wallet.Address,
                        VaultAddress = vault.Address,
                        Wallet = wallet,
                        CreatedAt = nowDay
                    };
                }

                if (vault.Balance == 0)
                {
                    vaultPosition!.ClosePosition(nowDay);
                }

                vaultPosition!.AddOrUpdateSnapshot(
                    new HyperliquidVaultPositionSnapshot(wallet, vault.Address, vault.Balance, nowDay));

                if (!cashFlowHistory.TryGetValue(vault.Address, out var cashFlowEvents))
                {
                    continue;
                }

                foreach (var cashFlowEvent in cashFlowEvents)
                {
                    vaultPosition.AddCashFlowIfNotExists(cashFlowEvent);
                }

                result.Add(vaultPosition);
            }
            catch
            {
                await _repository.UnitOfWork.RollbackTransactionAsync(ct);
                throw;
            }
        }

        await _repository.BulkMergeAsync(result, ct);
        await _repository.UnitOfWork.CommitTransactionAsync(ct);
    }
}