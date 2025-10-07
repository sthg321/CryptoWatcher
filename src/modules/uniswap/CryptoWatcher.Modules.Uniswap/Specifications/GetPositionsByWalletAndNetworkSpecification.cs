using Ardalis.Specification;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule.Entities;

namespace CryptoWatcher.Modules.Uniswap.Specifications;

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