namespace CryptoWatcher.Abstractions.PositionSnapshots;

public interface IPositionSnapshot
{
    DateOnly Day { get; init; }
}