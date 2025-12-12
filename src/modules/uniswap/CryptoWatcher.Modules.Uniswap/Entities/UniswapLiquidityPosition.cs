using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.Exceptions;
using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Entities;

/// <summary>
/// Represents a position in a liquidity pool, tracking the amounts and USD values of tokens,
/// as well as associated metadata such as creation date, uniswapNetwork information, and status.
/// </summary>
/// <remarks>
/// A liquidity pool position contains details about token quantities, their equivalent USD values,
/// and the associated blockchain uniswapNetwork. This class also includes information on whether the
/// position is currently active and the date it was created.
/// </remarks>
public class UniswapLiquidityPosition : ICalculatablePosition<ITokenPairPositionSnapshot>
{
    private readonly List<UniswapLiquidityPositionSnapshot> _positionSnapshots = [];
    private readonly List<UniswapLiquidityPositionCashFlow> _cashFlows = [];

    private UniswapLiquidityPosition()
    {
    }

    internal const string SameSymbolsException = "Can't create a position with same token symbols";

    internal const string InvalidTickRangeException = "Can't create a position with tick lower greater than tick upper";

    internal const string PositionClosedException = "Position already closed position";

    public UniswapLiquidityPosition(ulong positionId, long tickLower, long tickUpper, TokenInfo token0,
        TokenInfo token1, EvmAddress walletAddress, UniswapChainConfiguration chain, DateOnly createdAt)
    {
        if (token0.Symbol == token1.Symbol)
        {
            throw new DomainException(SameSymbolsException);
        }

        if (tickLower > tickUpper)
        {
            throw new DomainException(InvalidTickRangeException);
        }

        PositionId = positionId;
        TickLower = tickLower;
        TickUpper = tickUpper;
        Token0 = token0;
        Token1 = token1;
        WalletAddress = walletAddress;
        NetworkName = chain.Name;
        ProtocolVersion = chain.ProtocolVersion;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Represents the unique identifier for a liquidity pool position from NFT manager.
    /// </summary>
    /// <remarks>
    /// This property is used to uniquely identify a specific position within the liquidity pool.
    /// It serves as a key for referencing and managing individual positions across the system.
    /// </remarks>
    public ulong PositionId { get; private set; }

    /// <summary>
    /// Gets the lower tick boundary of the Uniswap liquidity position.
    /// </summary>
    public long TickLower { get; private set; }

    /// <summary>
    /// Represents the upper tick of an Uniswap liquidity position, delimiting the price range
    /// at which the position is active within the liquidity pool.
    /// </summary>
    public long TickUpper { get; private set; }

    /// <summary>
    /// Represents the first token in the liquidity pool pair for a position.
    /// </summary>
    /// <remarks>
    /// This property holds information about one of the two tokens in the liquidity pool.
    /// It is paired with <c>Token1</c> to define the token pair comprising the pool.
    /// </remarks>
    public TokenInfo Token0 { get; private set; } = null!;

    /// <summary>
    /// Represents the second token in a liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds details about the second token involved in the liquidity pool.
    /// It is used in association with <c>Token0</c> to define the token pair within the pool
    /// and their respective attributes, enabling operations such as valuation and tracking.
    /// </remarks>
    public TokenInfo Token1 { get; private set; } = null!;

    /// <summary>
    /// Indicates whether the liquidity pool position is active.
    /// </summary>
    /// <remarks>
    /// This property is used to determine the current status of the position.
    /// An active state signifies that the position is engaged in the liquidity pool,
    /// while an inactive state suggests the position has been exited or is no longer valid.
    /// </remarks>
    public bool IsClosed => ClosedAt is not null;

    /// <summary>
    /// When position created
    /// </summary>
    public DateOnly CreatedAt { get; private set; }
    
    /// <summary>
    /// When position closed
    /// </summary>
    public DateOnly? ClosedAt { get; private set; }

    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address that is linked to the liquidity pool position.
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
    public Wallet Wallet { get; init; } = null!;

    /// <summary>
    /// Specifies the name of the uniswapNetwork associated with the current configuration or operation.
    /// </summary>
    /// <remarks>
    /// This property is used to identify the uniswapNetwork, such as a blockchain or communication uniswapNetwork,
    /// that the system is interacting with or utilizing for its processes.
    /// </remarks>
    public string NetworkName { get; private set; } = null!;

    public UniswapProtocolVersion ProtocolVersion { get; private set; }

    /// <summary>
    /// Represents a collection of snapshots associated with a specific liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds historical states or snapshots of a liquidity pool position.
    /// These snapshots can be used to track changes and analyze the evolution of the position over time,
    /// including performance metrics, token balances, and other relevant data.
    /// </remarks>
    public IReadOnlyCollection<UniswapLiquidityPositionSnapshot> PoolPositionSnapshots => _positionSnapshots;

    public IReadOnlyCollection<UniswapLiquidityPositionCashFlow> CashFlows => _cashFlows;

    public UniswapLiquidityPositionSnapshot AddOrUpdateSnapshot(DateOnly day, bool isInRange, TokenInfoWithFee token0,
        TokenInfoWithFee token1)
    {
        if (IsClosed)
        {
            throw new DomainException(PositionClosedException);
        }
        
        var existedSnapshot = _positionSnapshots.FirstOrDefault(snapshot => snapshot.Day == day);
        if (existedSnapshot is null)
        {
            var snapshot = new UniswapLiquidityPositionSnapshot(this, day, isInRange, token0, token1);
            _positionSnapshots.Add(snapshot);
            return snapshot;
        }

        existedSnapshot.Update(token0, token1, isInRange);

        return existedSnapshot;
    }

    public UniswapLiquidityPositionCashFlow AddCashFlow(
        LiquidityPoolPositionEvent positionEvent,
        TokenInfoPair tokenInfoPair)
    {
        if (IsClosed)
        {
            throw new DomainException(PositionClosedException);
        }
        
        var existedSnapshot = _cashFlows.FirstOrDefault(snapshot => snapshot.Date == positionEvent.TimeStamp);
        if (existedSnapshot is null)
        {
            var cashFlow =
                new UniswapLiquidityPositionCashFlow(this, positionEvent, tokenInfoPair, positionEvent.TimeStamp);
            _cashFlows.Add(cashFlow);
            return cashFlow;
        }

        return existedSnapshot;
    }

    public void ClosePosition(DateOnly closedAt)
    {
        if (IsClosed)
        {
            throw new DomainException(PositionClosedException);
        }

        ClosedAt = closedAt;
    }

    public Money CalculateHoldValueInUsd(DateOnly to)
    {
        if (PoolPositionSnapshots.Count == 0)
        {
            return 0;
        }

        var lastPosition = PoolPositionSnapshots.GetNearestSnapshot(to, true);

        if (lastPosition is null)
        {
            return 0;
        }

        return Token0.Amount * lastPosition.Token0.PriceInUsd +
               Token1.Amount * lastPosition.Token1.PriceInUsd;
    }

    public Money CalculateFeeForPeriod(DateOnly from, DateOnly to)
    {
        var snapshotAtStart = PoolPositionSnapshots.GetLastSnapshotBefore(from);
        var snapshotAtEnd = PoolPositionSnapshots.GetLastSnapshotOnOrBefore(to);
    
        if (snapshotAtEnd is null) return 0;
    
        var feeFromPosition = snapshotAtEnd.FeeInUsd - (snapshotAtStart?.FeeInUsd ?? 0M);
        var feeFromCashFlows = CalculateDailyFeesFromCashFlows(from, to)
            .Values.SelectMany(x => x).Sum(c => c.FeeInUsd);
    
        return feeFromPosition + feeFromCashFlows;
    }

    public Money CalculateDailyFeeProfit(DateOnly day)
    {
        var todaySnapshot = PoolPositionSnapshots.GetLastSnapshotOnOrBefore(day);
    
        if (todaySnapshot is null)
            return 0;
    
        var prevSnapshot = PoolPositionSnapshots.GetLastSnapshotBefore(day);
    
        var feeFromPosition = todaySnapshot.FeeInUsd - (prevSnapshot?.FeeInUsd ?? 0M);
    
        var feeFromCashFlows = CalculateDailyFeesFromCashFlows(day, day)
            .Values.SelectMany(x => x).Sum(c => c.FeeInUsd);
    
        return feeFromPosition + feeFromCashFlows;
    }

    /// <summary>
    /// Calculates the total lifetime fees in USD for the position, including all claimed fees from cash flows
    /// and the current unclaimed fees from the latest snapshot.
    /// </summary>
    /// <remarks>
    /// This method assumes that PoolPositionSnapshots contain daily unclaimed fees (owed but not collected),
    /// and CashFlows record all historical fee claims (collections). The total is the sum of all claimed fees
    /// valued in USD at the time of claim (using the nearest snapshot's token prices) plus the unclaimed fees
    /// from the most recent snapshot. This provides the aggregate fees earned over the entire position lifetime,
    /// adhering to DDD principles by encapsulating calculation logic within the entity.
    /// If no snapshots exist, returns 0. Assumes GetNearestSnapshot extension handles finding the closest
    /// snapshot for price valuation, preferring exact matches or previous days if specified.
    /// </remarks>
    /// <returns>The total lifetime fees as a Money value in USD.</returns>
    public Money CalculateLifetimeTotalFeeInUsd(DateOnly to)
    {
        if (PoolPositionSnapshots.Count == 0)
        {
            return 0;
        }

        var lastSnapshot = PoolPositionSnapshots.GetNearestSnapshot(to, true);

        if (lastSnapshot == null)
        {
            return 0;
        }

        var claimedEvents = CalculateDailyFeesFromCashFlows(DateOnly.MinValue, to);

        return claimedEvents.Values
                   .SelectMany(flows => flows)
                   .Sum(flow => flow.Token0.Amount * lastSnapshot.Token0.PriceInUsd +
                                flow.Token1.Amount * lastSnapshot.Token1.PriceInUsd) +
               lastSnapshot.FeeInUsd;
    }

    private Dictionary<DateOnly, UniswapLiquidityPositionCashFlow[]> CalculateDailyFeesFromCashFlows(
        DateOnly from, DateOnly to)
    {
        return CashFlows
            .Where(flow => flow.Date.ToDateOnly() >= from && flow.Date.ToDateOnly() <= to)
            .Where(cashFlow => cashFlow.Event == CashFlowEvent.FeeClaim)
            .GroupBy(cashFlow => cashFlow.Date.ToDateOnly())
            .ToDictionary(
                g => g.Key,
                g => g.ToArray());
    }

    public IReadOnlyCollection<ICashFlow> GetCashFlows()
    {
        return CashFlows;
    }

    public IReadOnlyCollection<ITokenPairPositionSnapshot> GetPositionSnapshots()
    {
        return PoolPositionSnapshots;
    }
}