using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Models;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Api;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.UserNonFundingLedgerUpdates;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.UserVaultEquities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Services;

public class HyperliquidApiGateway : IHyperliquidGateway
{
    private readonly IHyperliquidApi _hyperliquidApi;

    public HyperliquidApiGateway(IHyperliquidApi hyperliquidApi)
    {
        _hyperliquidApi = hyperliquidApi;
    }

    public async Task<HyperliquidPositionCashFlow[]> GetCashFlowEventsAsync(Wallet wallet,
        DateTime from, DateTime to,
        CancellationToken ct = default)
    {
        var startTime = ((DateTimeOffset)from.AddDays(-2)).ToUnixTimeMilliseconds();
        var endTime = ((DateTimeOffset)to).ToUnixTimeMilliseconds();

        var result = await _hyperliquidApi.GetUserNonFundingLedgerUpdatesAsync(
            new UserNonFundingLedgerUpdatesRequest(wallet.Address, startTime, endTime), ct);

        return result
            .Where(update => update.Delta is VaultDeposit or VaultWithdraw)
            .Select(update => MapToVaultEvent(update, wallet))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<HyperliquidVault>> GetVaultsPositionsEquityAsync(Wallet wallet,
        CancellationToken ct = default)
    {
        var balance = await _hyperliquidApi.GetUserVaultEquitiesAsync(new UserVaultEquitiesRequest(wallet.Address), ct);

        return balance.Select(equity => new HyperliquidVault
        {
            Balance = equity.Equity,
            Address = EvmAddress.Create(equity.VaultAddress)
        }).ToArray();
    }

    private static HyperliquidPositionCashFlow MapToVaultEvent(UserNonFundingLedgerUpdate update, Wallet wallet)
    {
        var day = DateTime.UnixEpoch.AddMilliseconds(update.Time);
        return update.Delta switch
        {
            VaultDeposit vaultDeposit => new HyperliquidPositionCashFlow
            {
                Token0 = new CryptoTokenStatistic
                {
                    Amount = vaultDeposit.Usdc, PriceInUsd = 1
                },
                Event = CashFlowEvent.Deposit,
                VaultAddress = EvmAddress.Create(vaultDeposit.Vault),
                WalletAddress = wallet.Address,
                Date = day
            },
            VaultWithdraw vaultWithdraw => new HyperliquidPositionCashFlow
            {
                Token0 = new CryptoTokenStatistic
                {
                    Amount = vaultWithdraw.NetWithdrawnUsd, PriceInUsd = 1
                },
                Event = CashFlowEvent.Withdrawal,
                VaultAddress = EvmAddress.Create(vaultWithdraw.Vault),
                WalletAddress = wallet.Address,
                Date = day
            },
            _ => throw new ArgumentOutOfRangeException(nameof(update), update.Delta, null)
        };
    }
}