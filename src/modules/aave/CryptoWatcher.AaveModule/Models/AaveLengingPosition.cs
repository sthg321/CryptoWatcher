using System.Numerics;
using CryptoWatcher.AaveModule.Entities;
using Org.BouncyCastle.Crypto.Paddings;

namespace CryptoWatcher.AaveModule.Models;

// public abstract class AaveLendingPosition
// {
//     protected AaveLendingPosition(AaveNetwork network, string tokenAddress)
//     {
//         Network = network;
//         TokenAddress = tokenAddress;
//     }
//
//     public required AaveNetwork Network { get; init; }
//
//     public required string TokenAddress { get; init; }
// }
//
// public class EmptyAaveLendingPosition : AaveLendingPosition
// {
//     public EmptyAaveLendingPosition(AaveNetwork network, string tokenAddress) : base(network, tokenAddress)
//     {
//     }
// }
//
// public class BorrowedAaveLendingPosition : AaveLendingPosition
// {
//     public BorrowedAaveLendingPosition(AaveNetwork network, string tokenAddress,
//         BigInteger scaleAmount,
//         BigInteger variableBorrowIndex) : base(network, tokenAddress)
//     {
//         ScaleAmount = scaleAmount;
//         VariableBorrowIndex = variableBorrowIndex;
//     }
//
//     public required BigInteger ScaleAmount { get; init; }
//
//     public required BigInteger VariableBorrowIndex { get; init; }
// }
//
// public class SuppliedAaveLendingPosition : AaveLendingPosition
// {
//     public SuppliedAaveLendingPosition(AaveNetwork network, string tokenAddress, BigInteger scaleAmount,
//         BigInteger liquidityIndex) : base(network, tokenAddress)
//     {
//         ScaleAmount = scaleAmount;
//         LiquidityIndex = liquidityIndex;
//     }
//
//     public required BigInteger ScaleAmount { get; init; }
//
//     public required BigInteger LiquidityIndex { get; set; }
// }

public class AaveLendingPosition
{
    public required AaveNetwork Network { get; set; } = null!;

    public required BigInteger ScaleAmount { get; init; }

    public BigInteger? PoolIndex { get; init; }

    public required string TokenAddress { get; init; } = null!;

    public AavePositionType? PositionType { get; init; }

    public BigInteger CalculateAmountWithDebtOrFee()
    {
        if (PoolIndex is null)
        {
            throw new InvalidOperationException("Pool index is not set");
        }

        return ScaleAmount * PoolIndex.Value / BigInteger.Pow(10, 27);
    }

    public static AaveLendingPosition CreateEmpty(AaveNetwork network, string tokenAddress)
    {
        return new AaveLendingPosition
        {
            TokenAddress = tokenAddress,
            Network = network,
            ScaleAmount = 0,
            PositionType = null,
            PoolIndex = null
        };
    }
}