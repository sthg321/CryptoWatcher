using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.UniswapModule.Entities;

namespace CryptoWatcher.UniswapModule.Extensions;

internal static class TokenInfoPairExtensions
{
    public static TokenInfoPair NormalizeToPositionOrder(this TokenInfoPair eventTokenPair, PoolPosition dbPosition)
    {
        if (eventTokenPair.Token0.Symbol == dbPosition.Token0.Symbol &&
            eventTokenPair.Token1.Symbol == dbPosition.Token1.Symbol)
        {
            return eventTokenPair;
        }

        return eventTokenPair with { Token0 = eventTokenPair.Token1, Token1 = eventTokenPair.Token0 };
    }
}