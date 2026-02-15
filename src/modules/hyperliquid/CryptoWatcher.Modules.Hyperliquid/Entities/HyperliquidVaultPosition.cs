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

    private HyperliquidVaultPosition(EvmAddress vaultAddress, EvmAddress walletAddress)
    {
        Token0 = new CryptoToken
        {
            Amount = 0,
            Symbol = HyperliquidWellKnowFields.UsdcSymbol,
            PriceInUsd = HyperliquidWellKnowFields.UsdcPrice,
            Address = HyperliquidWellKnowFields.UsdcAddress
        };

        VaultAddress = vaultAddress;
        WalletAddress = walletAddress;
    }

    private readonly List<HyperliquidVaultPositionSnapshot> _snapshots = [];
    private readonly List<HyperliquidPositionCashFlow> _cashFlows = [];
    private readonly List<HyperliquidVaultPeriod> _periods = [];

    /// <summary>
    /// Represents the address of the vault associated with the Hyperliquid platform position.
    /// </summary>
    /// <remarks>
    /// This property holds the unique identifier of the vault on the blockchain.
    /// It is used to track and manage vault-specific data and operations within the Hyperliquid module.
    /// </remarks>
    public EvmAddress VaultAddress { get; private init; } = null!;

    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address linked to the liquidity pool position.
    /// It is used to identify the owner of the position and manage the related account details.
    /// </remarks>
    public EvmAddress WalletAddress { get; private init; } = null!;

    public CryptoToken Token0 { get; private init; } = null!;

    public HyperliquidVaultPeriod? ActivePeriod => Periods.FirstOrDefault(period => period.ClosedAt is null);

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

    public IReadOnlyCollection<HyperliquidVaultPeriod> Periods => _periods;

    public static HyperliquidVaultPosition Open(EvmAddress wallet, EvmAddress vault)
    {
        return new HyperliquidVaultPosition(vault, wallet);
    }

    public void OpenPeriod(DateTimeOffset startedAt)
    {
        if (ActivePeriod is not null)
        {
            throw new DomainException("Period is already open");
        }

        _periods.Add(HyperliquidVaultPeriod.StartNew(startedAt, WalletAddress, VaultAddress));
    }

    public void AddOrUpdateSnapshot(decimal amount, DateTime day)
    {
        if (amount == 0 && Snapshots.Count == 0)
        {
            throw new DomainException("Position cannot be closed without any snapshots or cash flows");
        }

        var existedSnapshot =
            _snapshots.FirstOrDefault(positionSnapshot => positionSnapshot.Day == DateOnly.FromDateTime(day));

        if (existedSnapshot is null)
        {
            _snapshots.Add(new HyperliquidVaultPositionSnapshot(WalletAddress, VaultAddress, amount,
                DateOnly.FromDateTime(day)));
            return;
        }

        existedSnapshot.UpdateFrom(amount);

        if (amount == 0)
        {
            ClosePeriod();
        }
    }

    public void AddCashFlowIfNotExists(decimal amount, CashFlowEvent @event, DateTimeOffset timestamp,
        TransactionHash hash)
    {
        if (ActivePeriod is null && @event == CashFlowEvent.Deposit)
        {
            OpenPeriod(timestamp);
        }

        if (ActivePeriod is null)
        {
            throw new DomainException(
                $"CashFlow {@event} cannot be applied without active period");
        }

        var exists = _cashFlows.Any(c => c.TransactionHash == hash);
        if (exists)
        {
            return;
        }

        _cashFlows.Add(new HyperliquidPositionCashFlow
        {
            Date = timestamp,
            Event = @event,
            Token0 = new CryptoTokenStatistic { Amount = amount, PriceInUsd = 1 },
            VaultAddress = VaultAddress,
            WalletAddress = WalletAddress,
            TransactionHash = hash
        });
    }

    private void ClosePeriod()
    {
        if (ActivePeriod is null)
        {
            throw new DomainException("Can't close position without open period");
        }

        var lastWithdraw = _cashFlows
            .Where(flow => flow.Date >= ActivePeriod.StartedAt && flow.Event == CashFlowEvent.Withdrawal)
            .MaxBy(flow => flow.Date);

        if (lastWithdraw is null)
        {
            throw new DomainException("Can't close position without withdrawals");
        }

        ActivePeriod.Close(lastWithdraw.Date);
    }
}