using CryptoWatcher.Extensions;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Entities;

public class AavePositionDailyPerformance
{
    private AavePositionDailyPerformance()
    {
    }

    /// <summary>
    /// Gets the unique identifier of the Aave position associated with this snapshot.
    /// </summary>
    /// <remarks>
    /// Serves as a reference to the specific Aave position that this snapshot is linked to.
    /// It is a part of the composite key (with the Day property) used to uniquely identify
    /// the snapshot and allows tracking of individual positions over time.
    /// </remarks>
    public Guid SnapshotPositionId { get; private set; }

    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address linked to the liquidity pool position.
    /// It is used to identify the owner of the position and manage the related account details.
    /// </remarks>
    public EvmAddress WalletAddress { get; private set; } = null!;

    /// <summary>
    /// Gets the specific day associated with the Aave position snapshot.
    /// </summary>
    /// <remarks>
    /// Represents the date for which this snapshot was taken. It is used as part of the
    /// composite key to uniquely identify the snapshot entry and can help in tracking
    /// changes or records daily.
    /// </remarks>
    public DateOnly Day { get; private set; }

    /// <summary>
    /// Gets or sets the name of the blockchain network associated with the Aave position.
    /// </summary>
    /// <remarks>
    /// Represents the specific network on which the Aave position exists, such as Ethereum or Polygon.
    /// This property is used to categorize and differentiate positions based on their network.
    /// </remarks>
    public string NetworkName { get; private set; } = null!;

    /// <summary>
    /// Specifies the type of position in the Aave protocol, such as whether it represents a supply or borrow activity.
    /// </summary>
    /// <remarks>
    /// This property is used to indicate and distinguish the nature of the position,
    /// whether the wallet has supplied or borrowed tokens within the Aave ecosystem.
    /// It plays a critical role in classifying and managing position-related data.
    /// </remarks>
    public AavePositionType PositionType { get; private set; }

    /// <summary>
    /// Gets the token information related to the Aave position snapshot.
    /// </summary>
    /// <remarks>
    /// Represents the specific token details captured in the snapshot, including properties such as
    /// the token's symbol, amount, and valuation. This data is essential for analyzing the financial
    /// state and performance of the associated Aave position at the time of the snapshot.
    /// </remarks>
    public CryptoToken Token0 { get; private set; } = null!; 

    /// <summary>
    /// Represents the profit, in USD, derived from a specific Aave position over the course of a day.
    /// </summary>
    /// <remarks>
    /// This property quantifies the change in the value of a position (measured in USD) on a given day,
    /// factoring in market price fluctuations, accrued interest, and other relevant metrics.
    /// It is calculated by comparing the USD value of the position across two snapshots
    /// and may either be positive (indicating gains) or negative (indicating losses).
    /// </remarks>
    public decimal ProfitInUsd { get; private set; }

    /// <summary>
    /// Gets the profit calculated in terms of the token's amount for the associated Aave position on a specific day.
    /// </summary>
    /// <remarks>
    /// Represents the net gain or loss in the token's quantity for a given day, as a result of changes in the Aave position.
    /// Useful for tracking performance in token units over time.
    /// </remarks>
    public decimal ProfitInToken { get; private set; }

    public static AavePositionDailyPerformance Create(AavePosition position, AavePositionSnapshot previous,
        AavePositionSnapshot current)
    {
        var profitInUsd = position.CalculateProfitInUsd(previous.Day, current.Day);
        var profitInToken = position.CalculateProfitInToken(previous.Day, current.Day);

        return new AavePositionDailyPerformance
        {
            Day = current.Day,
            WalletAddress = position.WalletAddress,
            NetworkName = position.Network,
            PositionType = position.PositionType,
            Token0 = position.Token0,
            ProfitInUsd = profitInUsd.Amount,
            ProfitInToken = profitInToken.Amount,
            SnapshotPositionId = current.PositionId
        };
    }
}