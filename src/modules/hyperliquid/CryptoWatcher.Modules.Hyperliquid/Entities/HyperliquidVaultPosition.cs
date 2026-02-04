using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Exceptions;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Entities;

/// <summary>
/// Represents a user's position within a Hyperliquid vault.
/// This class encapsulates various properties and methods to track and analyze the performance of the vault,
/// including event history, position snapshots, and profit calculations over specified time periods.
/// </summary>
public class
    HyperliquidVaultPosition : IDeFiPosition<HyperliquidVaultPositionSnapshot, HyperliquidPositionCashFlow>
{
    private HyperliquidVaultPosition()
    {
    }

    public HyperliquidVaultPosition(decimal balance, DateTime createdAt, EvmAddress vaultAddress,
        EvmAddress walletAddress)
    {
        Token0 = new CryptoToken
        {
            Amount = balance,
            Symbol = "USDC",
            PriceInUsd = 1,
            Address = EvmAddress.Create("0xaf88d065e77c8cC2239327C5EDb3A432268e5831")
        };

        CreatedAt = DateOnly.FromDateTime(createdAt);
        VaultAddress = vaultAddress;
        WalletAddress = walletAddress;
    }


    private readonly List<HyperliquidVaultPositionSnapshot> _snapshots = [];
    private readonly List<HyperliquidPositionCashFlow> _cashFlows = [];
    
    public DateOnly CreatedAt { get; private set; }

    public DateOnly? ClosedAt { get; private set; }

    public decimal InitialBalance { get; set; }

    /// <summary>
    /// Represents the address of the vault associated with the Hyperliquid platform position.
    /// </summary>
    /// <remarks>
    /// This property holds the unique identifier of the vault on the blockchain.
    /// It is used to track and manage vault-specific data and operations within the Hyperliquid module.
    /// </remarks>
    public EvmAddress VaultAddress { get; private set; } = null!;

    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address linked to the liquidity pool position.
    /// It is used to identify the owner of the position and manage the related account details.
    /// </remarks>
    public EvmAddress WalletAddress { get; private set; } = null!;

    /// <summary>
    /// Represents the wallet associated with a liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property identifies the wallet that holds ownership of the liquidity pool position.
    /// It includes the wallet's unique identifier and blockchain address for managing assets.
    /// </remarks>
    public Wallet Wallet { get; private set; } = null!;

    public CryptoToken Token0 { get; private set; } = null!;

    /// <summary>
    /// Represents the collection of events associated with the vault's activity.
    /// </summary>
    /// <remarks>
    /// This property maintains a list of historical events that occurred within the vault.
    /// Each event describes a specific action, such as deposits or withdrawals, along with relevant details.
    /// It is utilized to analyze vault performance, track cash flow, and compute metrics like percentage profit or rate of return.
    /// </remarks>
    public IReadOnlyCollection<HyperliquidPositionCashFlow> CashFlows => _cashFlows;

    /// <summary>
    /// Contains a collection of snapshots representing the states of a vault's position over time.
    /// </summary>
    /// <remarks>
    /// This property holds a list of <see cref="HyperliquidVaultPositionSnapshot"/> instances,
    /// where each snapshot captures specific details of the vault position at a given point in time.
    /// It is utilized for analysis, reporting, and tracking historical position data within the Hyperliquid module.
    /// </remarks>
    public IReadOnlyCollection<HyperliquidVaultPositionSnapshot> Snapshots => _snapshots;

    public static HyperliquidVaultPosition Open(
        EvmAddress wallet,
        EvmAddress vault,
        DateTime openedAt)
    {
        return new HyperliquidVaultPosition
        {
            WalletAddress = wallet,
            VaultAddress = vault,
            CreatedAt = DateOnly.FromDateTime(openedAt),
        };
    }

    public void AddOrUpdateSnapshot(decimal amount, DateOnly day)
    {
        var existedSnapshot =
            _snapshots.FirstOrDefault(positionSnapshot => positionSnapshot.Day == day);

        if (existedSnapshot is null)
        {
            _snapshots.Add(new HyperliquidVaultPositionSnapshot(WalletAddress, VaultAddress, amount, day));
            return;
        }

        existedSnapshot.UpdateFrom(amount);
    }
    
    public void AddOrUpdateSnapshot(HyperliquidVaultPositionSnapshot snapshot)
    {
        var existedSnapshot =
            _snapshots.FirstOrDefault(positionSnapshot => positionSnapshot.Day == snapshot.Day);

        if (existedSnapshot is null)
        {
            _snapshots.Add(snapshot);
            return;
        }

        existedSnapshot.UpdateFrom(snapshot);

        if (snapshot.Balance == 0)
        {
            ClosePosition(snapshot.Day);
        }
    }
 
    public void AddCashFlowIfNotExists(decimal amount, CashFlowEvent @event, DateTime timestamp)
    {
        var positionCashFlow = _cashFlows.FirstOrDefault(cashFlow => cashFlow.Date == timestamp &&
                                                                     cashFlow.Event == @event &&
                                                                     cashFlow.Token0.Amount ==
                                                                     amount);
        if (positionCashFlow is null)
        {
            _cashFlows.Add(new HyperliquidPositionCashFlow
            {
                Date = timestamp,
                Event = @event,
                Token0 = new CryptoTokenStatistic { Amount = amount, PriceInUsd = 1 },
                VaultAddress = VaultAddress,
                WalletAddress = WalletAddress,
            });
        }
    }

    private void ClosePosition(DateOnly closedAt)
    {
        if (ClosedAt.HasValue)
        {
            throw new DomainException("Cannot close closed position");
        }

        ClosedAt = closedAt;
    }
}