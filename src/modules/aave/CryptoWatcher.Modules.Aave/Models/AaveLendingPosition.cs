using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Models;

/// <summary>
/// Represents a base class for lending positions in the Aave protocol.
/// </summary>
/// <remarks>
/// This abstract class provides the foundational structure for various types of Aave lending positions,
/// including borrowed and supplied positions. Any specific lending position inherited from this base class.
/// </remarks>
public abstract class AaveLendingPosition
{
    public required EvmAddress TokenAddress { get; init; }
}