namespace CryptoWatcher.Abstractions;

public interface ICalculatablePosition<out TSnapshot> where TSnapshot : IPositionSnapshot
{
    IReadOnlyCollection<ICacheFlow> GetCashFlows();
    
    IReadOnlyCollection<TSnapshot> GetPositionSnapshots();
}