using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
using JetBrains.Annotations;

namespace CryptoWatcher.Modules.Aave.Entities;

/// <summary>
/// Represents a daily record of an Aave position, capturing key details about the position at a specific point in time.
/// </summary>
/// <remarks>
/// This snapshot includes the unique identifier for the associated position, the date the snapshot was taken,
/// and comprehensive token-related details such as its symbol, amount, and valuation.
/// </remarks>
public class AavePositionSnapshot : ITokenPositionSnapshot
{
    [UsedImplicitly] // for ef core
    private AavePositionSnapshot()
    {
    }

    public AavePositionSnapshot(Guid positionId, DateOnly day, CryptoTokenStatistic positionCryptoToken)
    {
        PositionId = positionId;
        Day = day;
        Token0 = positionCryptoToken;
    }

    /// <summary>
    /// Gets the unique identifier of the Aave position associated with this snapshot.
    /// </summary>
    /// <remarks>
    /// Serves as a reference to the specific Aave position that this snapshot is linked to.
    /// It is a part of the composite key (with the Day property) used to uniquely identify
    /// the snapshot and allows tracking of individual positions over time.
    /// </remarks>
    public Guid PositionId { get; init; }

    /// <summary>
    /// Gets the specific day associated with the Aave position snapshot.
    /// </summary>
    /// <remarks>
    /// Represents the date for which this snapshot was taken. It is used as part of the
    /// composite key to uniquely identify the snapshot entry and can help in tracking
    /// changes or records daily.
    /// </remarks>
    public DateOnly Day { get; init; }
  
    /// <summary>
    /// Gets the token information related to the Aave position snapshot.
    /// </summary>
    /// <remarks>
    /// Represents the specific token details captured in the snapshot, including properties such as
    /// the token's symbol, amount, and valuation. This data is essential for analyzing the financial
    /// state and performance of the associated Aave position at the time of the snapshot.
    /// </remarks>
    public CryptoTokenStatistic Token0 { get; private set; } = null!;
 
    public void UpdateToken(decimal amount, decimal priceInUsd)
    {
        Token0 = Token0 with { Amount = amount, PriceInUsd = priceInUsd };
    }
}