namespace CryptoWatcher.Abstractions.PositionSnapshots;

public interface IUsdPositionSnapshot : IPositionSnapshot
{
    decimal GetUsdBalance();
}