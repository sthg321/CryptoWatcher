using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Models;

public class UserReserve
{
    public required EvmAddress UnderlyingAsset { get; init; } = null!;

    public required BigInteger ScaledATokenBalance { get; init; }
 
    public required BigInteger ScaledVariableDebt { get; init; }
    
    public required bool IsCollateral { get; init; }
}