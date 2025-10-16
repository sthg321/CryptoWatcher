using System.Numerics;

namespace CryptoWatcher.Modules.Aave.Models;

/// <summary>
/// Indicates that the position is borrowed.
/// </summary>
public class BorrowedAaveLendingPosition : CalculatableAaveLendingPosition
{
    /// <summary>
    /// For positions with variable borrowed, this property represents the borrowed index
    /// </summary>
    public required BigInteger VariableBorrowIndex { get; init; }

    protected override BigInteger AccrualIndex => VariableBorrowIndex;
}