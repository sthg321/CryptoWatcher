using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services.PositionUpdates;

public class VaultUpdate
{
    public decimal Amount { get; init; }

    public DateTime Timestamp { get; init; }
    
    public EvmAddress VaultAddress { get; set; } = null!;

    public CashFlowEvent GetCashFlowEvent()
    {
        return this is DepositUpdate ? CashFlowEvent.Deposit : CashFlowEvent.Withdrawal;
    }
}