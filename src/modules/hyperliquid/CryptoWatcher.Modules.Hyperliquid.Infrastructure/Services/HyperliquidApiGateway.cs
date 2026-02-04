using System.Globalization;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Models;
using CryptoWatcher.Modules.Hyperliquid.Application.Services.PositionUpdates;
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

    public async Task<Queue<VaultUpdate>> GetVaultUpdatesAsync(EvmAddress walletAddress,
        DateTime from, DateTime to,
        CancellationToken ct = default)
    {
        var startTime = ((DateTimeOffset)from.AddDays(-2)).ToUnixTimeMilliseconds();
        var endTime = ((DateTimeOffset)to).ToUnixTimeMilliseconds();

        var result = await _hyperliquidApi.GetUserNonFundingLedgerUpdatesAsync(
            new UserNonFundingLedgerUpdatesRequest(walletAddress, startTime, endTime), ct);

        var stack = result
            .Where(update => update.Delta is VaultDeposit or VaultWithdraw)
            .Select(MapToVaultEvent)
            .OrderBy(update => update.Timestamp);

        return new Queue<VaultUpdate>(stack);
    }

    public async Task<IReadOnlyCollection<HyperliquidVault>> GetVaultsPositionsEquityAsync(EvmAddress walletAddress,
        CancellationToken ct = default)
    {
        var balance = await _hyperliquidApi.GetUserVaultEquitiesAsync(new UserVaultEquitiesRequest(walletAddress), ct);

        return balance.Select(equity => new HyperliquidVault
        {
            Balance = decimal.Parse(equity.Equity, CultureInfo.InvariantCulture),
            Address = EvmAddress.Create(equity.VaultAddress)
        }).ToArray();
    }

    private static VaultUpdate MapToVaultEvent(UserNonFundingLedgerUpdate update)
    {
        var timestamp = DateTime.UnixEpoch.AddMilliseconds(update.Time);
        return update.Delta switch
        {
            VaultDeposit vaultDeposit => new DepositUpdate
            {
                Amount = decimal.Parse(vaultDeposit.Usdc, CultureInfo.InvariantCulture),
                Timestamp = timestamp,
            },
            VaultWithdraw vaultWithdraw => new WithdrawUpdate
            {
                Amount = decimal.Parse(vaultWithdraw.NetWithdrawnUsd, CultureInfo.InvariantCulture),
                Timestamp = timestamp
            },
            _ => throw new ArgumentOutOfRangeException(nameof(update), update.Delta, null)
        };
    }
}