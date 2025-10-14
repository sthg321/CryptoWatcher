using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.AaveModule.Entities;

/// <summary>
/// Represents an event associated with an Aave position, providing details
/// about state changes such as deposits, withdrawals, and other position-related activities.
/// </summary>
/// <remarks>
/// This class is used to capture the transactional or state-change events within an Aave
/// position. Each event contains critical metadata including the identifier of the position,
/// the event type (e.g., deposit, withdrawal), the amount associated with the event, and
/// the timestamp when the event occurred. These details are essential for tracking the
/// lifecycle and changes made to an Aave position.
/// </remarks>
public class AavePositionEvent : ITokenCacheFlow
{
    /// <summary>
    /// Unique identifier for the position associated with the Aave position event.
    /// </summary>
    /// <remarks>
    /// This property serves as a required foreign key that links the Aave position events
    /// to a specific position in the system. It is used to maintain relational integrity
    /// between position events and their related positions.
    /// </remarks>
    public required Guid PositionId { get; init; }

    /// Represents the amount involved in the Aave position event.
    /// The value indicates the scaled amount of the asset being deposited or withdrawn.
    /// Positive values typically represent deposits, while negative values represent withdrawals.
    public required TokenInfo Token { get; init; }

    /// <summary>
    /// The date and time when the Aave position event occurred.
    /// </summary>
    /// <remarks>
    /// This property captures the precise timestamp of the event, which is used to
    /// track and sequence activities associated with a specific Aave position. It is
    /// a required component of the composite key that ensures uniqueness of events.
    /// </remarks>
    public required DateTime Date { get; init; }

    public CacheFlowEvent Event { get; init; } = null!;
}