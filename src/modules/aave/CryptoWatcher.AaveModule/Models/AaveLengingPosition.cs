using System.Numerics;
using CryptoWatcher.AaveModule.Entities;

namespace CryptoWatcher.AaveModule.Models;

public class AaveLendingPosition
{
    public required AaveNetwork Network { get; set; } = null!;
 
    public required BigInteger Amount { get; init; }

    public required string TokenAddress { get; init; } = null!;

    public AavePositionType? PositionType { get; init; }
}