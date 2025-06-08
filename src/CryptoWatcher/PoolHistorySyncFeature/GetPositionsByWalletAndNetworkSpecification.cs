using Ardalis.Specification;
using CryptoWatcher.Entities;
using CryptoWatcher.Entities.Uniswap;

namespace CryptoWatcher.PoolHistorySyncFeature;

public sealed class GetPositionsByWalletAndNetworkSpecification : Specification<PoolPosition>
{
    public GetPositionsByWalletAndNetworkSpecification(UniswapNetwork uniswapNetwork, Wallet wallet)
    {
        Query.Where(position => position.UniswapNetwork.Name == uniswapNetwork.Name && position.Wallet.Address == wallet.Address);
    }
}