using System.Numerics;
using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;

namespace CryptoWatcher.Modules.Aave.Application.Models;

/// <summary>
/// Indicate that position is borrowed or supplied.
/// </summary>
public abstract class CalculatableAaveLendingPosition : AaveLendingPosition
{
    private const int AaveIndexDecimals = 27;
    
    private static readonly BigInteger AaveIndexNormalizationFactor = BigInteger.Pow(10, AaveIndexDecimals);
     
    public required BigInteger ScaleAmount { get; init; }
    
    public required decimal TokenPriceInUsd { get; init; }
    
    public required byte TokenDecimals { get; init; }
    
    protected abstract BigInteger AccrualIndex { get; }

    public decimal CalculatePositionScaleInToken() => ScaleAmount.ToDecimal(TokenDecimals);
    
    public BigInteger CalculateAmountWithInterest() => ScaleAmount * AccrualIndex / AaveIndexNormalizationFactor;

    public decimal CalculateAmountWithInterestInUsd() =>
        (ScaleAmount * AccrualIndex / AaveIndexNormalizationFactor).ToDecimal(TokenDecimals) * TokenPriceInUsd;

    public AavePositionType DeterminePositionType() => this switch
    {
        BorrowedAaveLendingPosition _ => AavePositionType.Borrowed,
        SuppliedAaveLendingPosition _ => AavePositionType.Supplied,
        _ => throw new InvalidOperationException()
    };
}