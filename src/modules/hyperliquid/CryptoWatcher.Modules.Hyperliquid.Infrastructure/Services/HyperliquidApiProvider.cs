using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Modules.Hyperliquid.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserNonFundingLedgerUpdates.Contracts;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Services;

public class HyperliquidApiProvider : IHyperliquidProvider
{
    private readonly HyperliquidApiClient _client;

    public HyperliquidApiProvider(HyperliquidApiClient client)
    {
        _client = client;
    }

    public async Task<HyperliquidVaultEvent[]> GetVaultsFundingHistory(Wallet wallet,
        CancellationToken ct = default)
    {
        var result = await _client.UserNonFundingLedgerUpdates.GetUserNonFundingLedgerUpdates(wallet.Address, ct);
        
        return result
            .Where(update => update.Delta is VaultDeposit or VaultWithdraw)
            .Select(MapToVaultEvent)
            .ToArray();
    }

    public async Task<(string VaultAddress, decimal Equity)[]> GetVaultsPositionsEquityAsync(Wallet wallet,
        CancellationToken ct = default)
    {
        var balance = await _client.UserVaultEquities.GetUserVaultEquities(wallet.Address, ct);

        return balance.Select(equity => (equity.VaultAddress, equity.Equity)).ToArray();
    }
    
    private static HyperliquidVaultEvent MapToVaultEvent(UserNonFundingLedgerUpdate update)
    {
        var day = DateTime.UnixEpoch.AddMilliseconds(update.Time);
        return update.Delta switch
        {
            VaultDeposit vaultDeposit => new HyperliquidVaultEvent
            {
                Usd = vaultDeposit.Usdc,
                Event = CacheFlowEvent.Deposit,
                VaultAddress = EvmAddress.Create(vaultDeposit.Vault),
                Date = day
            },
            VaultWithdraw vaultWithdraw => new HyperliquidVaultEvent
            {
                Usd = vaultWithdraw.NetWithdrawnUsd,
                Event = CacheFlowEvent.Withdrawal,
                VaultAddress = EvmAddress.Create(vaultWithdraw.Vault),
                Date = day
            },
            _ => throw new ArgumentOutOfRangeException(nameof(update), update.Delta, null)
        };
    }
}