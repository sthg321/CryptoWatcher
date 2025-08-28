using CryptoWatcher.HyperliquidModule;
using CryptoWatcher.HyperliquidModule.Entities;
using CryptoWatcher.Shared.Entities;
using HyperliquidClient;
using HyperliquidClient.UserNonFundingLedgerUpdates.Contracts;

namespace CryptoWatcher.Host.Integrations;

public class HyperliquidProvider : IHyperliquidProvider
{
    private readonly HyperliquidApiClient _client;

    public HyperliquidProvider(HyperliquidApiClient client)
    {
        _client = client;
    }

    public async Task<HyperliquidVaultEvent[]> GetVaultsFundingHistory(Wallet wallet,
        CancellationToken ct = default)
    {
        var result = await _client.UserNonFundingLedgerUpdates.GetUserNonFundingLedgerUpdates(wallet.Address, ct);

        return result
            .Where(update => update.Delta is VaultDeposit or VaultWithdraw)
            .Select(update =>
            {
                var day = DateTime.UnixEpoch.AddMilliseconds(update.Time);
                return update.Delta switch
                {
                    VaultDeposit vaultDeposit => new HyperliquidVaultEvent
                    {
                        Usd = vaultDeposit.Usdc,
                        EventType = HyperliquidVaultVaultEventType.Deposit,
                        VaultAddress = vaultDeposit.Vault,
                        Date = day
                    },
                    VaultWithdraw vaultWithdraw => new HyperliquidVaultEvent
                    {
                        Usd = vaultWithdraw.NetWithdrawnUsd,
                        EventType = HyperliquidVaultVaultEventType.Withdrawal,
                        VaultAddress = vaultWithdraw.Vault,
                        Date = day
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(update.Delta), update.Delta, null)
                };
            })
            .ToArray();
    }

    public async Task<(string VaultAddress, decimal Equity)[]> GetVaultsPositionsEquityAsync(Wallet wallet,
        CancellationToken ct = default)
    {
        var balance = await _client.UserVaultEquities.GetUserVaultEquities(wallet.Address, ct);

        return balance.Select(equity => (equity.VaultAddress, equity.Equity)).ToArray();
    }
}