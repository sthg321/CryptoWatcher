using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Abstractions;

public interface IAaveGateway
{
    Task<IReadOnlyCollection<UserReserve>> GetUserReservesDataAsync(AaveProtocolConfiguration protocol,
        EvmAddress userAddress);

    Task<MarketReserveOutput> GetMarketReservesDataAsync(AaveProtocolConfiguration protocol);
}
