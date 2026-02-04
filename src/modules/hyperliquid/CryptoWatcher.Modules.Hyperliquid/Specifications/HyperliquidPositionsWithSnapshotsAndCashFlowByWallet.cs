using Ardalis.Specification;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Specifications;

public class HyperliquidPositionsWithSnapshotsAndCashFlowByWallet : Specification<HyperliquidVaultPosition>
{
    public HyperliquidPositionsWithSnapshotsAndCashFlowByWallet(EvmAddress walletAddress, DateOnly from, DateOnly to)
    {
        Query.Where(position => position.WalletAddress == walletAddress && !position.ClosedAt.HasValue)
            .Include(position =>
                position.Snapshots.Where(snapshot => snapshot.Day >= from && snapshot.Day <= to))
            .Include(position => position.CashFlows);
    }
}