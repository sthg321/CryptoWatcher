using CryptoWatcher.Abstractions;
using CryptoWatcher.Exceptions;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Entities;

/// <summary>
/// Represent market position from morpho.
/// Merket position contains borrowed asset and collateral token. 
/// </summary>
public class MorphoMarketPosition : IDeFiPosition<MorphoMarketPositionSnapshot, MorphoMarketPositionCashFlow>
{
    // ReSharper disable once CollectionNeverUpdated.Local 
    // support later
    private readonly List<MorphoMarketPositionCashFlow> _cashFlows = [];

    private readonly List<MorphoMarketPositionSnapshot> _snapshots = [];

    internal const string PositionClosedException = "Position already closed";
    internal const string ClosedAtGreatestThatCreatedAtException = "ClosedAt can't be greatest that CreatedAt";

    //for ef
    private MorphoMarketPosition()
    {
    }

    public MorphoMarketPosition(
        EvmAddress walletAddress,
        Guid marketExternalMarketExternalId,
        int chainId,
        CryptoToken loadToken,
        CryptoToken collateralToken,
        DateTime createdAt)
    {
        Id = Guid.CreateVersion7();
        WalletAddress = walletAddress;
        MarketExternalId = marketExternalMarketExternalId;
        ChainId = chainId;
        LoanToken = loadToken;
        CollateralToken = collateralToken;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    /// <summary>
    /// External id from morpho api.
    /// </summary>
    public Guid MarketExternalId { get; private set; }

    public int ChainId { get; private set; }

    public CryptoToken LoanToken { get; private set; } = null!;

    public CryptoToken CollateralToken { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    public DateTime? ClosedAt { get; private set; }

    public EvmAddress WalletAddress { get; private set; } = null!;

    public bool IsClosed => ClosedAt is not null;

    public IReadOnlyCollection<MorphoMarketPositionSnapshot> Snapshots => _snapshots;

    // support later
    public IReadOnlyCollection<MorphoMarketPositionCashFlow> CashFlows => _cashFlows;

    public void AddSnapshot(DateOnly day, CryptoTokenStatistic load, CryptoTokenStatistic collateralToken,
        double healthFactor, double liquidationLtv)
    {
        if (IsClosed)
        {
            throw new DomainException(PositionClosedException);
        }

        var existedSnapshot = Snapshots.FirstOrDefault(snapshot => snapshot.Day == day);
        if (existedSnapshot is not null)
        {
            existedSnapshot.UpdateSnapshot(load, collateralToken, healthFactor, liquidationLtv);
            return;
        }

        _snapshots.Add(new MorphoMarketPositionSnapshot(Id, day, load, collateralToken, healthFactor, liquidationLtv));
    }

    public void ClosePosition(DateTime closedAt)
    {
        if (closedAt < CreatedAt)
        {
            throw new DomainException(ClosedAtGreatestThatCreatedAtException);
        }

        if (IsClosed)
        {
            throw new DomainException(PositionClosedException);
        }

        ClosedAt = closedAt;
    }
}