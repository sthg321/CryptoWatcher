using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class PendingPositionEventBuffer
{
    private readonly Dictionary<ulong, List<UniswapEvent>> _events = new();

    public void AddEvent(ulong positionId, UniswapEvent uniswapEvent)
    {
        if (!_events.TryGetValue(positionId, out var events))
        {
            _events[positionId] = events = [];
        }

        events.Add(uniswapEvent);
    }

    public bool TryGetEventsForPosition(ulong positionId, out List<UniswapEvent>? events)
    {
        return _events.Remove(positionId, out events);
    }
}