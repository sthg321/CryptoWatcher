using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Models;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserNonFundingLedgerUpdates.Contracts;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Services;

public class HyperliquidApiProvider : IHyperliquidProvider
{
    private readonly IHyperliquidApiClient _client;

    public HyperliquidApiProvider(IHyperliquidApiClient client)
    {
        _client = client;
    }

    public async Task<HyperliquidVaultEvent[]> GetCashFlowEventsAsync(Wallet wallet,
        DateOnly from, DateOnly to,
        CancellationToken ct = default)
    {
        var result = await _client.UserNonFundingLedgerUpdates.GetUserNonFundingLedgerUpdates(wallet.Address,
            from.ToMinDateTime(), to.ToMaxDateTime(), ct);

        return result
            .Where(update => update.Delta is VaultDeposit or VaultWithdraw)
            .Select(update => MapToVaultEvent(update, wallet))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<HyperliquidVault>> GetVaultsPositionsEquityAsync(Wallet wallet,
        CancellationToken ct = default)
    {
        var balance = await _client.UserVaultEquities.GetUserVaultEquities(wallet.Address, ct);

        return balance.Select(equity => new HyperliquidVault
        {
            Balance = equity.Equity,
            Address = EvmAddress.Create(equity.VaultAddress)
        }).ToArray();
    }

    private static HyperliquidVaultEvent MapToVaultEvent(UserNonFundingLedgerUpdate update, Wallet wallet)
    {
        var day = DateTime.UnixEpoch.AddMilliseconds(update.Time);
        return update.Delta switch
        {
            VaultDeposit vaultDeposit => new HyperliquidVaultEvent
            {
                Usd = vaultDeposit.Usdc,
                Event = CashFlowEvent.Deposit,
                VaultAddress = EvmAddress.Create(vaultDeposit.Vault),
                WalletAddress = wallet.Address,
                Date = day
            },
            VaultWithdraw vaultWithdraw => new HyperliquidVaultEvent
            {
                Usd = vaultWithdraw.NetWithdrawnUsd,
                Event = CashFlowEvent.Withdrawal,
                VaultAddress = EvmAddress.Create(vaultWithdraw.Vault),
                WalletAddress = wallet.Address,
                Date = day
            },
            _ => throw new ArgumentOutOfRangeException(nameof(update), update.Delta, null)
        };
    }
}