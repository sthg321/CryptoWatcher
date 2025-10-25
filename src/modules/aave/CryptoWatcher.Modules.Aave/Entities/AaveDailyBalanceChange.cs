using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Entities;

public class AaveDailyBalanceChange
{
    /// <summary>
    /// Gets the unique identifier of the Aave position associated with this snapshot.
    /// </summary>
    /// <remarks>
    /// Serves as a reference to the specific Aave position that this snapshot is linked to.
    /// It is a part of the composite key (with the Day property) used to uniquely identify
    /// the snapshot and allows tracking of individual positions over time.
    /// </remarks>
    public Guid SnapshotPositionId { get; init; }

    /// <summary>
    /// Gets the specific day associated with the Aave position snapshot.
    /// </summary>
    /// <remarks>
    /// Represents the date for which this snapshot was taken. It is used as part of the
    /// composite key to uniquely identify the snapshot entry and can help in tracking
    /// changes or records daily.
    /// </remarks>
    public DateOnly Day { get; set; }

    /// <summary>
    /// Specifies the type of position in the Aave protocol, such as whether it represents a supply or borrow activity.
    /// </summary>
    /// <remarks>
    /// This property is used to indicate and distinguish the nature of the position,
    /// whether the wallet has supplied or borrowed tokens within the Aave ecosystem.
    /// It plays a critical role in classifying and managing position-related data.
    /// </remarks>
    public AavePositionType PositionType { get; set; }

    /// <summary>
    /// Gets the token information related to the Aave position snapshot.
    /// </summary>
    /// <remarks>
    /// Represents the specific token details captured in the snapshot, including properties such as
    /// the token's symbol, amount, and valuation. This data is essential for analyzing the financial
    /// state and performance of the associated Aave position at the time of the snapshot.
    /// </remarks>
    public TokenInfo Token { get; private set; } = null!;

    public static AaveDailyBalanceChange Create(AavePosition position, AavePositionSnapshot previous,
        AavePositionSnapshot current)
    {
        return new AaveDailyBalanceChange
        {
            Day = current.Day,
            PositionType = position.PositionType,
            SnapshotPositionId = current.PositionId,
            Token = current.Token
        };
    }
}