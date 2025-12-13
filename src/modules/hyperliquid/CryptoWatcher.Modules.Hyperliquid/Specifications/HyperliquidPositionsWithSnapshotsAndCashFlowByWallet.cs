using Ardalis.Specification;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Specifications;

public class HyperliquidPositionsWithSnapshotsAndCashFlowByWallet : Specification<HyperliquidVaultPosition>
{
    public HyperliquidPositionsWithSnapshotsAndCashFlowByWallet(Wallet wallet, DateOnly from, DateOnly to)
    {
        Query.Where(position => position.WalletAddress == wallet.Address && !position.ClosedAt.HasValue)
            .Include(position =>
                position.PositionSnapshots.Where(snapshot => snapshot.Day >= from && snapshot.Day <= to))
            .Include(position => position.CashFlows);
    }
}