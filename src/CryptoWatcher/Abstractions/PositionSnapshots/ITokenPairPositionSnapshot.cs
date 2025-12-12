using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Abstractions.PositionSnapshots;

public interface ITokenPairPositionSnapshot : IPositionSnapshot
{
    TokenInfoWithFee Token0 { get; }
    
    TokenInfoWithFee Token1 { get; }

    public decimal FeeAmountInUsd => Token0.FeeAmountInUsd + Token1.FeeAmountInUsd;
}