using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Services.PositionUpdates;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services;

public class HyperliquidVaultPositionUpdater
{
    private static readonly EvmAddress
        HlpVaultAddress = EvmAddress.Create("0xdfc24b077bc1425ad1dea75bcb6f8158e10df303");

    private readonly IHyperliquidGateway _gateway;

    public HyperliquidVaultPositionUpdater(IHyperliquidGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task<HyperliquidVaultPosition?> UpdatePositionAsync(
        HyperliquidVaultPosition? position,
        EvmAddress walletAddress,
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default)
    {
        var vault = (await _gateway.GetVaultsPositionsEquityAsync(walletAddress, ct))
            .FirstOrDefault(hyperliquidVault => hyperliquidVault.Address.Equals(HlpVaultAddress));

        if (position is null && vault is null)
        {
            return null;
        }

        var vaultUpdates = await _gateway.GetVaultUpdatesAsync(walletAddress, from.ToMinDateTime(), to.ToMaxDateTime(),
                ct);

        if (position is null)
        {
            if (vaultUpdates.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Vault exists for wallet {walletAddress} but no transaction history found in range [{from:yyyy-MM-dd}, {to:yyyy-MM-dd}]");
            }

            var firstUpdate = vaultUpdates.Peek();
            if (firstUpdate is not DepositUpdate)
            {
                throw new InvalidOperationException(
                    $"Invalid vault history for wallet {walletAddress}: expected first update to be Deposit, got {firstUpdate.GetType().Name}");
            }

            position = HyperliquidVaultPosition.Open(walletAddress, HlpVaultAddress, firstUpdate.Timestamp);
        }

        while (vaultUpdates.Count != 0)
        {
            var update = vaultUpdates.Dequeue();
            position.AddCashFlowIfNotExists(update.Amount, update.GetCashFlowEvent(), update.Timestamp);
        }

        position.AddOrUpdateSnapshot(vault?.Balance ?? 0, from);

        return position;
    }
}