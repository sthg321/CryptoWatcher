using CryptoWatcher.Abstractions.PositionSnapshots;

namespace CryptoWatcher.Extensions;

public static class PositionSnapshotExtensions
{
    public static TSnapshot? GetNearestSnapshot<TSnapshot>(
        this IReadOnlyCollection<TSnapshot> availableSnapshots, 
        DateOnly day,
        bool findPrevious) 
        where TSnapshot : IPositionSnapshot
    {
        if (availableSnapshots.Count == 0)
        {
            return default;
        }

        var daySnapshots = availableSnapshots.Where(s => s.Day == day).ToArray();
        if (daySnapshots.Length > 1)
        {
            return findPrevious 
                ? daySnapshots.MaxBy(s => s.Day)
                : daySnapshots.MinBy(s => s.Day);
        }

        return findPrevious
            ? availableSnapshots.Where(s => s.Day <= day).MaxBy(s => s.Day)
            : availableSnapshots.Where(s => s.Day >= day).MinBy(s => s.Day);
    }
}