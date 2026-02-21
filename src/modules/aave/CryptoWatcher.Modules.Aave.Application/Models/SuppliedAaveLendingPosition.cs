using System.Numerics;

namespace CryptoWatcher.Modules.Aave.Application.Models;

/// <summary>
/// Indicates that the position is supplied.
/// </summary>
public class SuppliedAaveLendingPosition : CalculatableAaveLendingPosition
{
    /// <summary>
    /// For positions with supplied liquidity, this property represents the liquidity index.
    /// </summary>
    public required BigInteger LiquidityIndex { get; init; }
    
    public required decimal LiquidationLtv { get; init; }

    protected override BigInteger AccrualIndex => LiquidityIndex;
    
    public required bool IsCollateral { get; init; }
}