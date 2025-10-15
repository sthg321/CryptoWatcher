using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Entities;

/// <summary>
/// Represents a user's position within a Hyperliquid vault.
/// This class encapsulates various properties and methods to track and analyze the performance of the vault,
/// including event history, position snapshots, and profit calculations over specified time periods.
/// </summary>
public class HyperliquidVaultPosition : ICalculatablePosition<IUsdPositionSnapshot>
{
    /// <summary>
    /// Represents the address of the vault associated with the Hyperliquid platform position.
    /// </summary>
    /// <remarks>
    /// This property holds the unique identifier of the vault on the blockchain.
    /// It is used to track and manage vault-specific data and operations within the Hyperliquid module.
    /// </remarks>
    public EvmAddress VaultAddress { get; init; } = null!;

    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address linked to the liquidity pool position.
    /// It is used to identify the owner of the position and manage the related account details.
    /// </remarks>
    public EvmAddress WalletAddress { get; init; } = null!;

    /// <summary>
    /// Represents the wallet associated with a liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property identifies the wallet that holds ownership of the liquidity pool position.
    /// It includes the wallet's unique identifier and blockchain address for managing assets.
    /// </remarks>
    public Wallet Wallet { get; init; } = null!;

    /// <summary>
    /// Represents the collection of events associated with the vault's activity.
    /// </summary>
    /// <remarks>
    /// This property maintains a list of historical events that occurred within the vault.
    /// Each event describes a specific action, such as deposits or withdrawals, along with relevant details.
    /// It is utilized to analyze vault performance, track cash flow, and compute metrics like percentage profit or rate of return.
    /// </remarks>
    public List<HyperliquidVaultEvent> VaultEvents { get; init; } = [];

    /// <summary>
    /// Contains a collection of snapshots representing the states of a vault's position over time.
    /// </summary>
    /// <remarks>
    /// This property holds a list of <see cref="HyperliquidVaultPositionSnapshot"/> instances,
    /// where each snapshot captures specific details of the vault position at a given point in time.
    /// It is utilized for analysis, reporting, and tracking historical position data within the Hyperliquid module.
    /// </remarks>
    public List<HyperliquidVaultPositionSnapshot> PositionSnapshots { get; init; } = [];

    public IReadOnlyCollection<IUsdPositionSnapshot> GetPositionSnapshots() => PositionSnapshots;

    public IReadOnlyCollection<ICacheFlow> GetCashFlows() => VaultEvents;
}