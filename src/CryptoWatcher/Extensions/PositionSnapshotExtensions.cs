using CryptoWatcher.Abstractions.PositionSnapshots;

namespace CryptoWatcher.Extensions;

public static class PositionSnapshotExtensions
{
    public static TSnapshot? GetNearestSnapshot<TSnapshot>(this IEnumerable<TSnapshot> snapshot, DateOnly day,
        bool findPrevious) where TSnapshot : IPositionSnapshot
    {
        return findPrevious
            ? snapshot.Where(s => s.Day <= day).MaxBy(s => s.Day)
            : snapshot.Where(s => s.Day >= day).MinBy(s => s.Day);
    }
}