using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Models;

/// <summary>
/// Represents a base class for lending positions in the Aave protocol.
/// </summary>
/// <remarks>
/// This abstract class provides the foundational structure for various types of Aave lending positions,
/// including borrowed and supplied positions. Any specific lending position inherited from this base class.
/// </remarks>
public class AaveLendingPosition
{
    public required EvmAddress TokenAddress { get; init; }

    public decimal PrincipalAmount { get; init; }

    public decimal Amount { get; init; }

    public decimal AmountUsd { get; init; }

    public decimal? LiquidationLtv { get; init; }

    public bool? IsCollateral { get; init; }

    public AavePositionType PositionType { get; set; }
}