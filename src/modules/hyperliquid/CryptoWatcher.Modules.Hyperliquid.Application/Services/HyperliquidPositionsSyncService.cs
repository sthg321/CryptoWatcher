using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Modules.Hyperliquid.Specifications;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services;

/// <summary>
/// <see cref="IHyperliquidPositionsSyncService"/>
/// </summary>
public class HyperliquidPositionsSyncService : IHyperliquidPositionsSyncService
{
    private readonly IHyperliquidProvider _hyperliquidProvider;
    private readonly IRepository<HyperliquidVaultPosition> _repository;
    private readonly TimeProvider _timeProvider;
    private readonly IHyperliquidSyncRepoFacade _facade;

    public HyperliquidPositionsSyncService(IHyperliquidProvider hyperliquidProvider,
        IRepository<HyperliquidVaultPosition> repository,
        TimeProvider timeProvider, IHyperliquidSyncRepoFacade facade)
    {
        _hyperliquidProvider = hyperliquidProvider;
        _repository = repository;

        _timeProvider = timeProvider;
        _facade = facade;
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

        var result = new List<HyperliquidVaultPosition>();

        foreach (var vault in hyperliquidVaultPositions)
        {
            try
            {
                var isPositionExist = existingPositionMap.TryGetValue(vault.Address, out var vaultPosition);
                if (!isPositionExist)
                {
                    vaultPosition = new HyperliquidVaultPosition
                    {
                        Token0 = new CryptoToken
                        {
                            Amount = vault.Balance,
                            Symbol = "USDC",
                            PriceInUsd = 1,
                            Address = EvmAddress.Create("0xaf88d065e77c8cC2239327C5EDb3A432268e5831")
                        },
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

                if (cashFlowHistory.TryGetValue(vault.Address, out var cashFlowEvents))
                {
                    foreach (var cashFlowEvent in cashFlowEvents)
                    {
                        vaultPosition.AddCashFlowIfNotExists(cashFlowEvent);
                    }
                }

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