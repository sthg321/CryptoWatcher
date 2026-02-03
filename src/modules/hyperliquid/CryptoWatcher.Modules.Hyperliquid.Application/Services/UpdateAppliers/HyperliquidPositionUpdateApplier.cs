using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Exceptions;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Services.PositionUpdates;
using CryptoWatcher.Modules.Hyperliquid.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services.UpdateAppliers;

public class HyperliquidPositionUpdateApplier : IHyperliquidPositionUpdateApplier
{
    public HyperliquidVaultPosition ApplyUpdate(HyperliquidVaultPosition? position, VaultUpdate update)
    {
        return update switch
        {
            DepositUpdate deposit => ApplyDeposit(position, deposit),
            WithdrawUpdate withdraw => ApplyWithdraw(position, withdraw),
            _ => throw new DomainException($"Unknown update type: {update.GetType().Name}")
        };
    }

    private static HyperliquidVaultPosition ApplyDeposit(HyperliquidVaultPosition? position, DepositUpdate update)
    {
        if (position is null)
        {
            return new HyperliquidVaultPosition(
                update.Amount,
                update.Timestamp,
                update.VaultAddress,
                update.WalletAddress);
        }

        position.AddCashFlowIfNotExists(update.Amount, CashFlowEvent.Deposit, update.Timestamp);
        return position;
    }

    private static HyperliquidVaultPosition ApplyWithdraw(HyperliquidVaultPosition? position, WithdrawUpdate update)
    {
        if (position is null)
        {
            throw new DomainException("Position can't be null for withdraw event");
        }

        position.AddCashFlowIfNotExists(update.Amount, CashFlowEvent.Withdrawal, update.Timestamp);
        return position;
    }
}