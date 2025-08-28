using Ardalis.Specification;
using CryptoWatcher.Entities;
using CryptoWatcher.Entities.Uniswap;

namespace CryptoWatcher.PoolHistorySyncFeature;

public sealed class GetPositionsByWalletAndNetworkSpecification : Specification<PoolPosition>
{
    public GetPositionsByWalletAndNetworkSpecification(UniswapNetwork uniswapNetwork, Wallet wallet)
    {
        Query
            .Include(position => position.PoolPositionSnapshots)
            .Where(position => position.UniswapNetwork.Name == uniswapNetwork.Name &&
                               position.Wallet.Address == wallet.Address &&
                               position.IsActive);
    }
}